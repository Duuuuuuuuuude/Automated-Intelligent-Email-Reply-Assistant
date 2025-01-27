using AIERA.AIEmailClient.Configurations.EmailReply;
using AIERA.AIEmailClient.RegularExpression;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Globalization;
using System.Text;
using Multipart = MimeKit.Multipart;


namespace AIERA.AIEmailClient;

/// <summary>
/// A visitor that constructs a reply to a <see cref="MimeMessage"/>.
/// </summary>
public class ReplyVisitor : MimeVisitor
{
    /// <summary>
    /// The reply that was constructed by the visitor.
    /// </summary>
    public MimeMessage? Reply { get; private set; }

    /// <summary>
    /// The original message that the reply is in response to.
    /// </summary>
    private MimeMessage? _original;

    private readonly Stack<Multipart> _stack = new();

    /// <summary>
    /// The mailbox address of the person replying to the message.
    /// </summary>
    private readonly MailboxAddress _from;

    /// <summary>
    /// Whether or not the reply should be sent to all of the original recipients.
    /// </summary>
    private readonly bool _replyToAll;

    /// <summary>
    /// The text that should be used as the body of the reply.
    /// </summary>
    private readonly string _replyBodyText;
    private readonly EmailReplyConfig _emailReplyConfig;

    /// <summary>
    /// Used to construct a new <see cref="ReplyVisitor"/>,
    /// </summary>
    /// <param name="from"> The mailbox address of the person replying to the message.</param>
    /// <param name="replyToAll">Whether or not the reply should be sent to all of the original recipients.</param>
    /// <param name="replyText"> The text that should be used as the body of the reply.</param>
    /// <param name="emailReplyOptions"></param>
    public ReplyVisitor(MailboxAddress from, bool replyToAll, string replyText, IOptions<EmailReplyConfig> emailReplyOptions)
    {
        _from = from;
        _replyToAll = replyToAll;
        _replyBodyText = replyText;
        _emailReplyConfig = emailReplyOptions.Value;
    }


    private void Push(MimeEntity entity)
    {
        if (Reply!.Body == null)
        {
            Reply.Body = entity;
        }
        else
        {
            var parent = _stack.Peek();
            parent.Add(entity);
        }

        if (entity is Multipart multipart)
            _stack.Push(multipart);
    }

    private void Pop() => _stack.Pop();


    /// <summary>
    /// Get the 'On {DATE}, {SENDER} wrote:' string, that can be used to quote the original message.
    /// </summary>
    /// <param name="message">This messages envelope properties are used to create the returned string with.</param>
    /// <returns>A string in the format 'On {DATE}, {SENDER} wrote:'</returns>
    private static string GetOnDateSenderWrote(MimeMessage message)
    {
        var sender = message.Sender ?? message.From.Mailboxes.FirstOrDefault();
        var name = sender != null ? (!string.IsNullOrEmpty(sender.Name) ? sender.Name : sender.Address) : "an unknown sender";

        return string.Format(CultureInfo.InvariantCulture, "On {0}, {1} wrote:", message.Date.ToString("f", CultureInfo.CurrentCulture), name);
    }


    /// <summary>
    /// Visit the specified message.
    /// </summary>
    /// <param name="message">The message being replied to.</param>
    public override void Visit(MimeMessage message)
    {
        Reply = new MimeMessage();
        _original = message;

        _stack.Clear();

        Reply.From.Add(_from.Clone());

        AddPrimaryRecipients(message);
        AddOriginalReplyRecipients(message);
        RemoveSenderFromOriginalRecipients();
        AddReplySubject(message);
        ConstructInReplyToAndReferencesHeaders(message);

        base.Visit(message);
    }

    /// <summary>
    /// Makes sure the sender of the message will be replied to/sets the primary recipients.
    /// </summary>
    /// <param name="message"></param>
    private void AddPrimaryRecipients(MimeMessage message)
    {
        if (message.ReplyTo.Count > 0)
            Reply!.To.AddRange(message.ReplyTo);
        else if (message.From.Count > 0)
            Reply!.To.AddRange(message.From);
        else if (message.Sender != null)
            Reply!.To.Add(message.Sender);
    }

    /// <summary>
    /// Includes all of the other original recipients in the 'To' and 'Cc' lists.
    /// </summary>
    /// <param name="message"></param>
    private void AddOriginalReplyRecipients(MimeMessage message)
    {
        if (_replyToAll)
        {
            Reply!.To.AddRange(message.To);
            Reply.Cc.AddRange(message.Cc);
        }
    }

    /// <summary>
    /// Remove ourselves from the 'Cc' and 'To' lists.
    /// </summary>
    private void RemoveSenderFromOriginalRecipients()
    {
        if (_replyToAll)
        {
            // TODO: Remove ourselves from the 'Cc' and 'To' lists.
            //Reply!.To.Remove(ourselves));
            //Reply.Cc.Remove(ourselves));
        }
    }

    /// <summary>
    /// Set the reply subject to '<see cref="EmailReplyConfig.SubjectRepliedPrefix"/> {original subject}'.
    /// </summary>
    /// <param name="message"></param>
    private void AddReplySubject(MimeMessage message)
    {
        if (!RegexPatterns.IsRepliedEmailSubjectRegex().IsMatch(message.Subject.Trim())) // Regex pattern matches 'Re[#]:', 'Re:', 'Sv:', 'FW:' or 'FWD:' at the beginning of the string.
            Reply!.Subject = $"{_emailReplyConfig.SubjectRepliedPrefix} {message.Subject ?? string.Empty}";
        else
            Reply!.Subject = message.Subject;
    }

    /// <summary>
    /// Construct the In-Reply-To and References headers.
    /// </summary>
    /// <param name="message"></param>
    private void ConstructInReplyToAndReferencesHeaders(MimeMessage message)
    {
        if (!string.IsNullOrEmpty(message.MessageId))
        {
            Reply!.InReplyTo = message.MessageId;
            foreach (string? id in message.References)
                Reply.References.Add(id);
            Reply.References.Add(message.MessageId);
        }
    }

    /// <summary>
    /// Visit the specified entity.
    /// </summary>
    /// <param name="entity">The MIME entity.</param>
    /// <exception cref="System.NotSupportedException">
    /// Only Visit(MimeMessage) is supported.
    /// </exception>
    public override void Visit(MimeEntity entity)
    {
        throw new NotSupportedException();
    }

    protected override void VisitMultipartAlternative(MultipartAlternative alternative)
    {
        var multipart = new MultipartAlternative();

        Push(multipart);

        for (int i = 0; i < alternative.Count; i++)
            alternative[i].Accept(this);

        Pop();
    }

    protected override void VisitMultipartRelated(MultipartRelated related)
    {
        var multipart = new MultipartRelated();
        var root = related.Root;

        Push(multipart);

        root.Accept(this);

        for (int i = 0; i < related.Count; i++)
        {
            if (related[i] != root)
                related[i].Accept(this);
        }

        Pop();
    }

    protected override void VisitMultipart(Multipart multipart)
    {
        foreach (var part in multipart)
        {
            if (part is MultipartAlternative)
                part.Accept(this);
            else if (part is MultipartRelated)
                part.Accept(this);
            else if (part is TextPart)
                part.Accept(this);
        }
    }

    private void HtmlTagCallback(HtmlTagContext ctx, HtmlWriter htmlWriter)
    {
        if (ctx.TagId == HtmlTagId.Body && !ctx.IsEmptyElementTag)
        {
            if (ctx.IsEndTag)
            {
                // end our opening <blockquote>
                htmlWriter.WriteEndTag(HtmlTagId.BlockQuote);

                // pass the </body> tag through to the output
                ctx.WriteTag(htmlWriter, writeAttributes: true);
            }
            else
            {
                // pass the <body> tag through to the output
                ctx.WriteTag(htmlWriter, writeAttributes: true);

                // prepend the HTML reply with "On {DATE}, {SENDER} wrote:"
                htmlWriter.WriteStartTag(HtmlTagId.P);
                htmlWriter.WriteText(GetOnDateSenderWrote(_original!));
                htmlWriter.WriteEndTag(HtmlTagId.P);

                // Wrap the original content in a <blockquote>
                htmlWriter.WriteStartTag(HtmlTagId.BlockQuote);
                htmlWriter.WriteAttribute(HtmlAttributeId.Style, "border-left: 1px #ccc solid; margin: 0 0 0 .8ex; padding-left: 1ex;");

                ctx.InvokeCallbackForEndTag = true;
            }
        }
        else
        {
            // pass the tag through to the output
            ctx.WriteTag(htmlWriter, writeAttributes: true);
        }
    }

    private string QuoteText(string text)
    {
        using var quoted = new StringWriter();

        quoted.WriteLine(GetOnDateSenderWrote(_original!));

        using (var reader = new StringReader(text))
        {
            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                quoted.Write("> ");
                quoted.WriteLine(line);
            }
        }

        return quoted.ToString();
    }


    // TODO: Add this instead of "On {DATE}, {SENDER} wrote:"
    //  <b>Fra:</b> Bob Alice<example@outlook.dk>
    //  <b>Sendt:</b> 23. januar 2025 16:46
    //  <b>Til:</b> Alice Bob <example2@outlook.dk>
    //  <b>Emne:</b> test 30 subject...
    //
    //  <Body goes here.>
    protected override void VisitTextPart(TextPart entity)
    {
        string text;

        if (entity.IsHtml)
        {
            var converter = new HtmlToHtml
            {
                HtmlTagCallback = HtmlTagCallback,
            };

            text = converter.Convert(entity.Text);
        }
        else if (entity.IsFlowed)
        {
            var converter = new FlowedToText();

            text = converter.Convert(entity.Text);
            text = QuoteText(text);
        }
        else
        {
            // quote the original message text
            text = QuoteText(entity.Text);
        }

        var part = new TextPart(entity.ContentType.MediaSubtype.ToLowerInvariant())
        {
            Text = new StringBuilder().Append($"<b/>{_emailReplyConfig.AIReplyBodyPrefix}</b>")
                                      .AppendLine("<br/><br/>")
                                      .Append(_replyBodyText)
                                      .AppendLine(text)
                                      .ToString(),
        };

        Push(part);
    }

    protected override void VisitMessagePart(MessagePart entity)
    {
        // don't descend into message/rfc822 parts
    }
}
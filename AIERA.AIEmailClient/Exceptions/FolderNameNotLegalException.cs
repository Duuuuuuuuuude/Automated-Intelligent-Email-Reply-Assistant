namespace AIERA.AIEmailClient.Exceptions;


public class FolderNameNotLegalException : Exception
{
    /// <summary>
    /// Path of the folder not created.
    /// </summary>
    public string FolderPath { get; private set; }

    private static string DefaultMessageFormat { get; } = ("The folder path '{0}' does not contain a legal folder name.");

    /// <summary>
    /// Exception thrown when a folder name is not legal according to the email server creating the folder.
    /// </summary>
    /// <param name="folderPathNotFound"></param>
    public FolderNameNotLegalException(string folderPathNotFound) : base(string.Format(DefaultMessageFormat, folderPathNotFound))
    {
        FolderPath = folderPathNotFound;
    }


    /// <summary>
    /// Exception thrown when a folder name is not legal according to the email server creating the folder.
    /// </summary>
    /// <param name="folderPathNotFound"></param>
    /// <param name="message"></param>
    public FolderNameNotLegalException(string folderPathNotFound, string message) : base(message)
    {
        FolderPath = folderPathNotFound;
    }

    /// <summary>
    /// Exception thrown when a folder name is not legal according to the email server creating the folder.
    /// </summary>
    /// <param name="folderPathNotFound"></param>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public FolderNameNotLegalException(string folderPathNotFound, string message, Exception innerException) : base(message, innerException)
    {
        FolderPath = folderPathNotFound;
    }

    /// <summary>
    /// Exception thrown when a folder name is not legal according to the email server creating the folder.
    /// </summary>
    /// <param name="folderPathNotFound"></param>
    /// <param name="innerException"></param>
    public FolderNameNotLegalException(string folderPathNotFound, Exception innerException) : base(string.Format(DefaultMessageFormat, folderPathNotFound), innerException)
    {
        FolderPath = folderPathNotFound;
    }

    public override string ToString()
    {
        return string.Format("{0}: {1}\n FolderPathNotFound='{2}''\n{3}",
                             GetType().Name,
                             base.Message,
                             FolderPath,
                             StackTrace);
    }
}
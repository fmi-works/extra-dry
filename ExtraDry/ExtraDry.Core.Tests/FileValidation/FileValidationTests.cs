﻿namespace ExtraDry.Core.Tests;

public class FileValidationTests {

    public FileValidationTests()
    {
        options = new FileValidationOptions() {
            CheckMagicBytesAndMimes = true,
            FileDatabaseLocation = "FileUpload/FileDatabase.json"
        };

        service = new FileValidationService(options);

        validator = new FileValidator(service);
    }

    [Theory]
    [InlineData("good.txt")]
    [InlineData("Resumè4.docx")]
    [InlineData("Caractères-accentués.txt")]
    [InlineData("Sedím-na-stole-a-moja-výška-mi-umožňuje-pohodlne-čítať-knihu.txt")]
    [InlineData("myArchive.tar.gz")]
    public void DoesNotAlterGoodFileNames(string inputFilename)
    {
        var clean = service.CleanFilename(inputFilename);

        Assert.Equal(inputFilename, clean);
    }

    [Theory]
    [InlineData("file1.test-1.txt", "file1.test-1.txt")]
    [InlineData("file1-.test-1.txt", "file1.test-1.txt")]
    [InlineData("123___sad---.html", "123_sad.html")]
    [InlineData("Caractères[]@%$#&().txt", "Caractères.txt")]
    [InlineData("Caractères[]@%$#&()sifd.txt", "Caractères-sifd.txt")]
    [InlineData("[]@%$#&()Caractèressifd.txt", "Caractèressifd.txt")]
    [InlineData("Sedím na stole a moja výška mi umožňuje pohodlne čítať knihu.txt", "Sedím-na-stole-a-moja-výška-mi-umožňuje-pohodlne-čítať-knihu.txt")]
    [InlineData("_siudh_.txt", "_siudh.txt")]
    [InlineData("-siudh-.txt", "siudh.txt")]
    [InlineData("-21938721309781231content.txt", "21938721309781231content.txt")]
    public void AltersFileNameToCorrect(string inputFilename, string expected)
    {
        var clean = service.CleanFilename(inputFilename);

        Assert.Equal(expected, clean);
    }

    [Theory(Skip = "Refactor to remove file dependency")]
    [InlineData("text.txt", "text/plain")]
    [InlineData("jpg.jpg", "image/jpeg")]
    [InlineData("png.png", "image/png")]
    [InlineData("rtf.rtf", "text/rtf")]
    [InlineData("word.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")] // interesting use case. .docx magic bytes = .zip magic bytes = .apk magic bytes
    [InlineData("zip.zip", "application/zip")] // interesting use case. .docx magic bytes = .zip magic bytes = .apk magic bytes
    [InlineData("Tiff.tiff", "image/tiff")]
    [InlineData("doc.doc", "application/msword")]
    [InlineData("mp4.mp4", "audio/mp4")]
    [InlineData("mp4.mp4", "audio/aac")]
    [InlineData("mp4.mp4", "video/mp4")]
    [InlineData("html.html", "text/html")]
    public void ValidToUploadFiles(string filename, string mime)
    {
        var fileBytes = File.ReadAllBytes($"FileUpload/SampleFiles/{filename}");

        validator.ValidateFile(filename, mime, fileBytes);

        Assert.True(validator.IsValid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("!bat.txt")]
    [InlineData("te1--xt.txt")]
    [InlineData("j%pg.txt")]
    [InlineData("rtf-.rtf")]
    [InlineData("text")]
    public void InvalidFilename(string filename)
    {
        var validator = OptionedFileValidator(config => {
                config.ValidateFilename = ValidationCondition.Always;
                config.ValidateExtension = ValidationCondition.Never;
                config.ValidateContent = ValidationCondition.Never;
            });

        validator.ValidateFile(filename, "text/plain");

        Assert.False(validator.IsValid);
    }

    [Theory(Skip = "Refactor to remove file dependency")]
    [InlineData("doc.docx", "jpg.jpg", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    [InlineData("file.txt", "png.png", "text/plain")]
    public void MismatchingNameAndBytes(string filename, string filepath, string mime)
    {
        var fileBytes = File.ReadAllBytes($"FileUpload/SampleFiles/{filepath}");

        validator.ValidateFile(filename, mime, fileBytes);

        Assert.False(validator.IsValid);
        Assert.Throws<ValidationException>(() => validator.ThrowIfNotValid());
    }

    [Theory(Skip = "Refactor to remove file dependency")]
    [InlineData("doc.docx", "word.docx", "image/jpeg")]
    [InlineData("file.txt", "text.txt", "image/jpeg")]
    public void MismatchingNameAndMime(string filename, string filepath, string mime)
    {
        var fileBytes = File.ReadAllBytes($"FileUpload/SampleFiles/{filepath}");

        validator.ValidateFile(filename, mime, fileBytes);

        Assert.False(validator.IsValid);
        var exception = Assert.Throws<ValidationException>(() => validator.ThrowIfNotValid());
        Assert.Contains("filename and mime type do not match", exception.Message);
    }

    [Theory(Skip = "Refactor to remove file dependency")]
    [InlineData("bat.bat", "bat.bat", "text/plain")]
    [InlineData("exe.exe", "exe.exe", "application/x-dosexec")]
    [InlineData("zip.jar", "zip.zip", "application/java-archive")]
    [InlineData("file.apk", "word.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    public void BlacklistFileType(string filename, string filepath, string mime)
    {
        var fileBytes = File.ReadAllBytes($"FileUpload/SampleFiles/{filepath}");

        validator.ValidateFile(filename, mime, fileBytes);

        Assert.False(validator.IsValid);
        var exception = Assert.Throws<ValidationException>(() => validator.ThrowIfNotValid());
    }

    [Theory(Skip = "Refactor to remove file dependency")]
    [InlineData("html.html", "htmlScript.html", "textScript/html")] // Has script tags
    public void XmlFileWithScriptFileType(string filename, string filepath, string mime)
    {
        var fileBytes = File.ReadAllBytes($"FileUpload/SampleFiles/{filepath}");

        validator.ValidateFile(filename, mime, fileBytes);

        Assert.False(validator.IsValid);
        Assert.Throws<ValidationException>(() => validator.ThrowIfNotValid());
    }

    [Theory(Skip = "Refactor to remove file dependency")]
    [InlineData("txt", "text.txt", "text.txt", "text/plain")] 
    [InlineData("docx", "word.docx", "word.docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    [InlineData("docx", "zip.zip", "zip.zip", "application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    public void CanConfigureBlacklist(string blackListFileExtension, string filepath, string filename, string mime)
    {
        options = new FileValidationOptions() {
            ExtensionWhitelist = new List<string> { "txt", "jpg", "png", "rtf", "docx", "docm", "tiff", "doc", "mp4", "html", "zip" },
            ExtensionBlacklist = new List<BlacklistFileType>() { new() { Extension = blackListFileExtension } },
            CheckMagicBytesAndMimes = true,
            FileDatabaseLocation = "FileUpload/FileDatabase.json"
        };
        service = new FileValidationService(options);
        validator = new FileValidator(service);

        var fileBytes = File.ReadAllBytes($"FileUpload/SampleFiles/{filepath}");
        validator.ValidateFile(filename, mime, fileBytes);

        Assert.False(validator.IsValid);
        Assert.Throws<ValidationException>(() => validator.ThrowIfNotValid());
    }

    [Theory(Skip = "Refactor to remove file dependency")]
    [InlineData("docx", "word.docx", "zip.zip", "application/zip")]
    public void NameOnlyBlacklistDoesntCheckBytes(string blackListFileExtension, string filepath, string filename, string mime)
    {
        options = new FileValidationOptions() {
            ExtensionBlacklist = new List<BlacklistFileType>() { new() { Extension = blackListFileExtension, CheckType = CheckType.FilenameOnly } },
            CheckMagicBytesAndMimes = true,
            FileDatabaseLocation = "FileUpload/FileDatabase.json"
        };
        service = new FileValidationService(options);
        validator = new FileValidator(service);
        var fileBytes = File.ReadAllBytes($"FileUpload/SampleFiles/{filepath}");

        validator.ValidateFile(filename, mime, fileBytes);

        Assert.True(validator.IsValid);
    }


    [Theory(Skip = "Refactor to remove file dependency")]
    [InlineData("exe.exe", "exe.jpg", "image/jpg")]
    [InlineData("exe.exe", "exe.jpg", "application/x-dosexec")]
    public void ClientSideConfigDoesntCheckBytes(string filepath, string filename, string mime)
    {
        options = new FileValidationOptions() {
            CheckMagicBytesAndMimes = false,
        };
        service = new FileValidationService(options);
        validator = new FileValidator(service);
        var fileBytes = File.ReadAllBytes($"FileUpload/SampleFiles/{filepath}");

        validator.ValidateFile(filename, mime, fileBytes);

        Assert.True(validator.IsValid);
    }

    [Theory(Skip = "Refactor to remove file dependency")]
    [InlineData("jpg", "exe.exe", "exe.jpg", "image/jpg")]
    [InlineData("exe", "exe.exe", "exe.exe", "application/x-dosexec")]
    public void ClientSideConfigDoesRejectByName(string blackListFileExtension, string filepath, string filename, string mime)
    {
        options = new FileValidationOptions() {
            ExtensionBlacklist = new List<BlacklistFileType>() { new() { Extension = blackListFileExtension, CheckType = CheckType.FilenameOnly } },
            CheckMagicBytesAndMimes = false,
            ValidateContent = ValidationCondition.Never,
            ValidateExtension = ValidationCondition.Always,
        };
        service = new FileValidationService(options);
        validator = new FileValidator(service);
        var fileBytes = File.ReadAllBytes($"FileUpload/SampleFiles/{filepath}");

        validator.ValidateFile(filename, mime, fileBytes);

        Assert.False(validator.IsValid);
        var exception = Assert.Throws<ValidationException>(() => validator.ThrowIfNotValid());
    }

    [Theory(Skip = "Refactor to remove file dependency")]
    [InlineData("exe.exe", "exe.jpg", "image/jpg")]
    [InlineData("exe.exe", "exe.html", "application/x-dosexec")]
    public void ClientSideConfigDoesAcceptValidFilenames(string filepath, string filename, string mime)
    {
        options = new FileValidationOptions() {
            CheckMagicBytesAndMimes = false,
            ValidateContent = ValidationCondition.Never,
        };
        service = new FileValidationService(options);
        validator = new FileValidator(service);
        var fileBytes = File.ReadAllBytes($"FileUpload/SampleFiles/{filepath}");

        validator.ValidateFile(filename, mime, fileBytes);

        Assert.True(validator.IsValid);
    }

    private FileValidationService service;

    private FileValidationOptions options;

    private FileValidator validator;

    private static FileValidator OptionedFileValidator(Action<FileValidationOptions> config)
    {
        var service = OptionedFileValidationService(config);
        return new FileValidator(service);
    }

    private static FileValidationService OptionedFileValidationService(Action<FileValidationOptions> config)
    {
        var options = new FileValidationOptions();
        config(options);
        return new FileValidationService(options);
    }

}

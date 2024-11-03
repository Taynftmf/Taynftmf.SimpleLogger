namespace Taynftmf.SimpleLogger.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using Xunit;
    using Taynftmf;

    public class SimpleLoggerTests
    {
        private const string TestLogFile = "test.log";
        private const string ConfigFileName = "log-setup.txt";
        private readonly string _configFilePath;
        private readonly string _logFilePath;

        public SimpleLoggerTests()
        {
            _configFilePath = Path.Combine(Path.GetTempPath(), ConfigFileName);
            _logFilePath = Path.Combine(Path.GetTempPath(), TestLogFile);
            File.Delete(_logFilePath);
        }

        [Fact]
        public void Log_ShouldNotWrite_WhenConfigFileDoesNotExist()
        {
            // Arrange
            if (File.Exists(_configFilePath))
            {
                File.Delete(_configFilePath);
            }

            // Act
            SimpleLogger.Log("Test message");

            // Assert
            Assert.False(File.Exists(_logFilePath));
        }

        [Fact]
        public void Log_ShouldNotWrite_WhenConfigFileIsEmpty()
        {
            // Arrange
            File.WriteAllText(_configFilePath, "");

            // Act
            SimpleLogger.Log("Test message");

            // Assert
            Assert.False(File.Exists(TestLogFile));
        }

        [Fact]
        public void Log_ShouldWriteMessage_WhenKeywordsMatch()
        {
            // Arrange
            File.WriteAllLines(_configFilePath, new[] { TestLogFile, "keyword1", "keyword2" });

            // Act
            SimpleLogger.Log("Test message", "keyword1");

            // Assert
            var logContent = File.ReadAllText(TestLogFile);
            Assert.Contains("Test message", logContent);
            Assert.Contains("keyword1", logContent);
        }

        [Fact]
        public void Log_ShouldNotWriteMessage_WhenNoKeywordsMatch()
        {
            // Arrange
            File.WriteAllLines(_configFilePath, new[] { TestLogFile, "keyword1", "keyword2" });

            // Act
            SimpleLogger.Log("Test message", "nonexistent");

            // Assert
            var logContent = File.Exists(TestLogFile) ? File.ReadAllText(TestLogFile) : string.Empty;
            Assert.DoesNotContain("Test message", logContent);
        }

        [Fact]
        public void Log_ShouldAppendToLogFile_WhenCalledMultipleTimes()
        {
            // Arrange
            File.WriteAllLines(_configFilePath, new[] { TestLogFile, "keyword1" });
            
            // Act
            SimpleLogger.Log("First message", "keyword1");
            SimpleLogger.Log("Second message", "keyword1");

            // Assert
            var logContent = File.ReadAllLines(TestLogFile);
            Assert.Equal(2, logContent.Length);
            Assert.Contains("First message", logContent[0]);
            Assert.Contains("Second message", logContent[1]);
        }

        [Fact]
        public void Dispose()
        {
            if (File.Exists(TestLogFile))
                File.Delete(TestLogFile);
            
            if (File.Exists(_configFilePath))
                File.Delete(_configFilePath);
        }
    }
}
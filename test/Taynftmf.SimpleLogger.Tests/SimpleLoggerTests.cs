namespace Taynftmf.SimpleLogger.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using Xunit;
    using Taynftmf;

    public class SimpleLoggerTests
    {
        private readonly string _configFilePath;
        private readonly string _logFilePath;

        public SimpleLoggerTests()
        {
            _configFilePath = Path.Combine(Path.GetTempPath(), "log-setup.txt");
            _logFilePath = Path.Combine(Path.GetTempPath(), "test.log");
            File.Delete(_configFilePath);
            File.Delete(_logFilePath);
        }

        [Fact]
        public void Log_ShouldNotWrite_WhenConfigFileDoesNotExist()
        {
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
            Assert.False(File.Exists(_logFilePath));
        }

        [Fact]
        public void Log_ShouldWriteMessage_WhenKeywordsMatch()
        {
            // Arrange
            File.WriteAllLines(_configFilePath, new[] { _logFilePath, "keyword1", "keyword2" });

            // Act
            SimpleLogger.Log("Test message", "keyword1");

            // Assert
            var logContent = File.ReadAllText(_logFilePath);
            Assert.Contains("Test message", logContent);
            Assert.Contains("keyword1", logContent);
        }

        [Fact]
        public void Log_ShouldWriteMessage_WhenNoKeywordsGiven()
        {
            // Arrange
            File.WriteAllLines(_configFilePath, new[] { _logFilePath });

            // Act
            SimpleLogger.Log("Test message", "keyword1");
            SimpleLogger.Log("Test message2", "keyword2");

            // Assert
            Assert.Equal(2, File.ReadAllLines(_logFilePath).Length);
            string logContent = File.ReadAllText(_logFilePath);
            Assert.Contains("keyword1", logContent);
            Assert.Contains("keyword2", logContent);
        }

        [Fact]
        public void Log_ShouldNotWriteMessage_WhenNoKeywordsMatch()
        {
            // Arrange
            File.WriteAllLines(_configFilePath, new[] { _logFilePath, "keyword1", "keyword2" });

            // Act
            SimpleLogger.Log("Test message", "nonexistent");

            // Assert
            var logContent = File.Exists(_logFilePath) ? File.ReadAllText(_logFilePath) : string.Empty;
            Assert.DoesNotContain("Test message", logContent);
        }

        [Fact]
        public void Log_ShouldAppendToLogFile_WhenCalledMultipleTimes()
        {
            // Arrange
            File.WriteAllLines(_configFilePath, new[] { _logFilePath, "keyword1" });
            
            // Act
            SimpleLogger.Log("First message", "keyword1");
            SimpleLogger.Log("Second message", "keyword1");
            SimpleLogger.Log("Ignored", "keyword2");

            // Assert
            Assert.Equal(2, File.ReadAllLines(_logFilePath).Length);
            var logContent = File.ReadAllText(_logFilePath);
            Assert.Contains("First message", logContent);
            Assert.Contains("Second message", logContent);
        }

        [Fact]
        public void Dispose()
        {
            //File.Delete(_logFilePath);
            //File.Delete(_configFilePath);
        }
    }
}
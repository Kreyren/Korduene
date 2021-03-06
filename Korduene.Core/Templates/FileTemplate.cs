﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Korduene.Templates
{
    /// <summary>
    /// File Template
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    [Description("FileTemplate")]
    [DisplayName("FileTemplate")]
    [DebuggerDisplay("{Name}")]
    public class FileTemplate : INotifyPropertyChanged
    {
        #region [Events]

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region [Private Objects]

        private string _name;
        private string _extension;
        private string _defaultName;
        private string _chosenName;
        private string _namespace;
        private string _language;
        private List<string> _files;
        private string _file;
        private string _icon;
        private string _mainFile;

        #endregion

        #region [Public Properties]

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        /// <value>
        /// The extension.
        /// </value>
        public string Extension
        {
            get { return _extension; }
            set
            {
                if (_extension != value)
                {
                    _extension = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the default name.
        /// </summary>
        /// <value>
        /// The default name.
        /// </value>
        public string DefaultName
        {
            get { return _defaultName; }
            set
            {
                if (_defaultName != value)
                {
                    _defaultName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the chosen.
        /// </summary>
        /// <value>
        /// The name of the chosen.
        /// </value>
        [JsonIgnore]
        public string ChosenName
        {
            get { return _chosenName; }
            set
            {
                if (_chosenName != value)
                {
                    _chosenName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        /// <value>
        /// The namespace.
        /// </value>
        [JsonIgnore]
        public string Namespace
        {
            get { return _namespace; }
            set
            {
                if (_namespace != value)
                {
                    _namespace = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Language
        {
            get { return _language; }
            set
            {
                if (_language != value)
                {
                    _language = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the main file.
        /// </summary>
        /// <value>
        /// The main file.
        /// </value>
        public string MainFile
        {
            get { return _mainFile; }
            set
            {
                if (_mainFile != value)
                {
                    _mainFile = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the files.
        /// </summary>
        /// <value>
        /// The files.
        /// </value>
        public List<string> Files
        {
            get { return _files ??= new List<string>(); }
            set
            {
                if (_files != value)
                {
                    _files = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>
        /// The file.
        /// </value>
        [JsonIgnore]
        public string File
        {
            get { return _file; }
            set
            {
                if (_file != value)
                {
                    _file = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>
        /// The icon.
        /// </value>
        public string Icon
        {
            get { return _icon; }
            set
            {
                if (_icon != value)
                {
                    _icon = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region [Commands]

        #endregion

        #region [Constructors]

        /// <summary>
        /// Initializes a new instance of the <see cref="File"/> class.
        /// </summary>
        public FileTemplate()
        {
        }

        #endregion

        #region [Public Methods]

        public string Serialize()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true });
        }

        public void CreateFiles(string destinationDir)
        {
            if (!System.IO.Directory.Exists(destinationDir))
            {
                System.IO.Directory.CreateDirectory(destinationDir);
            }

            var templateDir = System.IO.Path.GetDirectoryName(File);

            var files = new List<string>(Files)
            {
                this.MainFile
            };

            foreach (var file in files)
            {
                var fullPath = System.IO.Path.Combine(templateDir, file);
                var destinationPath = System.IO.Path.Combine(destinationDir, GetDestinationFileName(file));
                System.IO.File.Copy(fullPath, destinationPath, true);
                System.IO.File.WriteAllText(destinationPath, ReplaceKeywords(System.IO.File.ReadAllText(destinationPath)));
            }
        }

        #endregion

        #region [Public Static Methods]

        public static FileTemplate GetTemplate(string file)
        {
            var template = JsonSerializer.Deserialize<FileTemplate>(System.IO.File.ReadAllText(file));
            template.File = file;

            return template;
        }

        public static IEnumerable<FileTemplate> GetTemplates(string directory)
        {
            var result = new List<FileTemplate>();

            foreach (var file in new System.IO.DirectoryInfo(directory).GetFiles("*.json", System.IO.SearchOption.AllDirectories).Where(x => x.Name.Equals("template.json")))
            {
                result.Add(GetTemplate(file.FullName));
            }

            return result;
        }

        #endregion

        #region [Private Methods]

        private string GetDestinationFileName(string file)
        {
            //TODO: templates should have their own parameters
            return file.Replace("FileName", ChosenName);
        }

        private string ReplaceKeywords(string content)
        {
            //TODO: templates should have their own parameters
            return content.Replace("FileName", ChosenName).Replace("Namespace", Namespace);
        }

        #endregion

        /// <summary>
        /// Called when public properties changed.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

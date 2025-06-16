using System.IO.Compression;
using Chroma;
using Chroma.Audio.Sources;
using Chroma.ContentManagement;
using Chroma.Diagnostics.Logging;
using Chroma.Graphics;
using Chroma.Graphics.TextRendering.TrueType;
using Chroma.MemoryManagement;

namespace MelonEngine
{
    public class ZipContentProvider : DisposableResource, IContentProvider
    {
        private Dictionary<Type, Func<string, object[], object>> _importers = new();

        private Game _game;
        private List<ZipFile> _loadedFiles = new();

        private Log Log { get; } = LogManager.GetForCurrentAssembly();
        private string[] CommandLine { get; } = Environment.CommandLine.Split(" ");

        public string ContentRoot { get; }

        public ZipContentProvider(Game game, string zipFile)
        {
            Log.Info($"Mounting root filesystem, located at {zipFile}");

            if (!File.Exists(zipFile))
                throw new RootfsNotFoundException(zipFile);

            ContentRoot = zipFile;
            _game = game;
            ZipFile _file = new ZipFile() { ZipFileStream = new FileStream(zipFile, FileMode.Open) };
            _file.ZipArchive = new ZipArchive(_file.ZipFileStream, ZipArchiveMode.Read);
            _loadedFiles.Add(_file);

            foreach (var entry in _file.ZipArchive.Entries)
            {
                if (entry.FullName.EndsWith('/'))
                    continue;

                Log.Debug(entry.FullName);
            }

            if (Environment.CommandLine.Split(" ").Contains("-file") && CommandLine.Length > Array.IndexOf(CommandLine, "-file") + 1)
            {
                for (int i = Array.IndexOf(CommandLine, "-file") + 1; i <= CommandLine.Length - 1; i++)
                {
                    if (!File.Exists(CommandLine[i]))
                        break;
                    string _modPath = PathDetector.IsPathAbsolute(CommandLine[i]) ? CommandLine[i] : Path.Combine(AppContext.BaseDirectory, CommandLine[i]);
                    Log.Info($"Mounting overlayfs #{i - Array.IndexOf(CommandLine, "-file")}, located at {_modPath}");
                    _file = new ZipFile() { ZipFileStream = new FileStream(_modPath, FileMode.Open) };
                    _file.ZipArchive = new ZipArchive(_file.ZipFileStream, ZipArchiveMode.Read);
                    _loadedFiles.Add(_file);
                    foreach (var entry in _file.ZipArchive.Entries)
                    {
                        if (entry.FullName.EndsWith('/'))
                            continue;

                        Log.Debug(entry.FullName);
                    }
                }
            }

            _importers.Add(typeof(Texture), (path, _) =>
            {
                using (var stream = Open(path))
                {
                    using (var ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        
                        return new Texture(ms);
                    }
                }
            });

            _importers.Add(typeof(Sound), (path, _) =>
            {
                using (var stream = Open(path))
                {
                    using (var ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        
                        return new Sound(ms);
                    }
                }
            });

            _importers.Add(typeof(Music), (path, _) =>
            {
                var stream = Open(path);
                var ms = new MemoryStream();
                stream.CopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);

                return new Music(ms);
            });
            
            _importers.Add(typeof(TrueTypeFont), (path, args) =>
            {
                using (var stream = Open(path))
                {
                    using (var ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        ms.Seek(0, SeekOrigin.Begin);

                        if (args.Length == 2)
                        {
                            return new TrueTypeFont(ms, (int)args[0], (string)args[1]);
                        }
                        else if (args.Length == 1)
                        {
                            return new TrueTypeFont(ms, (int)args[0]);
                        }
                        else
                        {
                            return new TrueTypeFont(ms, 12);
                        }
                    }
                }
            });
        }

        public T Load<T>(string relativePath, params object[] args) where T : DisposableResource
        {
            return _importers[typeof(T)](relativePath, args) as T;
        }

        public void Unload<T>(T resource) where T : DisposableResource
        {
            throw new NotImplementedException();
        }

        public void Track<T>(T resource) where T : DisposableResource
        {
            throw new NotImplementedException();
        }

        public void StopTracking<T>(T resource) where T : DisposableResource
        {
            throw new NotImplementedException();
        }

        public byte[] Read(string relativePath)
        {
            foreach (ZipFile _file in Enumerable.Reverse(_loadedFiles))
            {
                if (_file.ZipArchive.GetEntry(relativePath) == null)
                    continue;

                using (var stream = _file.ZipArchive.GetEntry(relativePath)?.Open())
                {
                    using (var ms = new MemoryStream())
                    {
                        stream?.CopyTo(ms);
                        return ms.ToArray();
                    }
                }
            }

            return null;
        }

        public Stream Open(string relativePath)
        {
            foreach (ZipFile _file in Enumerable.Reverse(_loadedFiles))
            {
                if (_file.ZipArchive.GetEntry(relativePath) != null)
                {
                    return _file.ZipArchive.GetEntry(relativePath)?.Open();
                }
            }

            return null;
        }

        public void RegisterImporter<T>(Func<string, object[], object> importer) where T : DisposableResource
        {
            var contentType = typeof(T);

            if (_importers.ContainsKey(contentType))
            {
                throw new InvalidOperationException(
                    $"An importer for type {contentType.Name} was already registered."
                );
            }

            _importers.Add(contentType, importer);
        }

        public void UnregisterImporter<T>() where T : DisposableResource
        {
            throw new NotImplementedException();
        }

        public bool IsImporterPresent<T>() where T : DisposableResource
        {
            throw new NotImplementedException();
        }
    }

    public class ZipFile
    {
        public FileStream ZipFileStream { get; set; }
        public ZipArchive ZipArchive { get; set; }
    }

    public class RootfsNotFoundException : Exception
    {
        public RootfsNotFoundException(string path) : base(String.Format("Root archive not found at {0}", path))
        {
        }
    }
}
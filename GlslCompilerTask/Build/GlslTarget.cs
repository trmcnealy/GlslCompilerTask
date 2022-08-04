

//using Microsoft.Build.Framework;
//using Microsoft.Build.Utilities;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Diagnostics;


//namespace InlineCode
//{

//    public class VertexShaderTask : Microsoft.Build.Utilities.Task {
        
//        private Microsoft.Build.Framework.ITaskItem[] _Files;
        
//        public virtual Microsoft.Build.Framework.ITaskItem[] Files {
//            get {
//                return _Files;
//            }
//            set {
//                _Files = value;
//            }
//        }
        
//        private string _Configuration;
        
//        public virtual string Configuration {
//            get {
//                return _Configuration;
//            }
//            set {
//                _Configuration = value;
//            }
//        }
        
//        private Microsoft.Build.Framework.ITaskItem[] _OutputFiles;
        
//        public virtual Microsoft.Build.Framework.ITaskItem[] OutputFiles {
//            get {
//                return _OutputFiles;
//            }
//            set {
//                _OutputFiles = value;
//            }
//        }
        
//        private string _Errors;
        
//        public virtual string Errors {
//            get {
//                return _Errors;
//            }
//            set {
//                _Errors = value;
//            }
//        }
        
//        private bool _Success = true;
        
//        public virtual bool Success {
//            get {
//                return _Success;
//            }
//            set {
//                _Success = value;
//            }
//        }
        
//        public override bool Execute() {

//            ProcessStartInfo glslangValidatorStartInfo;
//            Process          glslangValidatorProcess;
//            ProcessStartInfo spirv_crossStartInfo;
//            Process          spirv_crossProcess;
            
//            OutputFiles = new TaskItem[Files.Length];
//            Errors = string.Empty;
            
//            string debug = (Configuration == "Release") ? "-O" : "-g";
            
//            ITaskItem item;
//            string sourceFileDir;
//            string sourceFilePath;
//            string sourceFileName;
//            string sprivFilePath;

//            string target_env = "opengl"; //"vulkan1.2";
            
//            string vulkansdk_path = Environment.GetEnvironmentVariable("VULKANSDK_PATH");
            
//            if(string.IsNullOrEmpty(vulkansdk_path))
//            {
//                Log.LogError("Environment variable VULKANSDK_PATH is missing. Find the Sdk here...https://vulkan.lunarg.com/sdk/home");
//                return false;
//            }
            
//            string glslangValidatorExePath = Path.Combine(vulkansdk_path, "bin", "glslangValidator.exe");
//            string spirvcrossExePath = Path.Combine(vulkansdk_path, "bin", "spirv-cross.exe");

//            for (int i = 0; i < Files.Length; i++)
//            {
//                item           = Files[i];
//                sourceFilePath = item.GetMetadata("FullPath"); //.Replace("\\", "/");
//                sourceFileDir  = Path.GetDirectoryName(sourceFilePath);
//                sourceFileName = Path.GetFileNameWithoutExtension(sourceFilePath);
//                sprivFilePath  = sourceFilePath + ".spv"; //.Replace("\\", "/");
//                OutputFiles[i] = new TaskItem(sprivFilePath);
                
//                glslangValidatorStartInfo = new ProcessStartInfo
//                {
//                    //glslangValidator
//                    FileName               = glslangValidatorExePath,
//                    // --auto-map-locations
//                    Arguments              = "-I" + sourceFileDir + " -t -S vert --glsl-version 330 --auto-map-locations --target-env " + target_env + " " + debug + " " + sourceFilePath + " -o " + sprivFilePath,
//                    RedirectStandardOutput = true,
//                    RedirectStandardError  = true,
//                    UseShellExecute        = false,
//                    CreateNoWindow         = true
//                };
                
//                Log.LogMessage(MessageImportance.High, glslangValidatorStartInfo.FileName + " " + glslangValidatorStartInfo.Arguments);

//                Errors += glslangValidatorStartInfo.FileName + " " + glslangValidatorStartInfo.Arguments;

//                glslangValidatorProcess = new Process();
//                glslangValidatorProcess.StartInfo = glslangValidatorStartInfo;                
//                glslangValidatorProcess.EnableRaisingEvents = true;
//                glslangValidatorProcess.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
//                {
//                    if (!string.IsNullOrEmpty(e.Data))
//                    {
//                        //Log.LogMessage(MessageImportance.High, e.Data);
                        
//                        if(e.Data != sourceFilePath)
//                        {
//                            Log.LogError(e.Data);
//                        }
//                        Errors += e.Data;
//                    }
//                });
//                glslangValidatorProcess.ErrorDataReceived  += new DataReceivedEventHandler((sender, e) =>
//                {
//                    if (!string.IsNullOrEmpty(e.Data))
//                    {
//                        Log.LogError(e.Data);
//                        Errors += e.Data;
//                    }
//                });
//                glslangValidatorProcess.Start();                
//                glslangValidatorProcess.BeginOutputReadLine();
//                glslangValidatorProcess.BeginErrorReadLine();            
//                glslangValidatorProcess.WaitForExit();
//                glslangValidatorProcess.Close();

//                spirv_crossStartInfo = new ProcessStartInfo
//                {
//                    FileName               = spirvcrossExePath,
//                    Arguments              = "--cpp-interface-name " + sourceFileName + " " + sprivFilePath + " --output " + sprivFilePath + ".cpp",
//                    RedirectStandardOutput = true,
//                    RedirectStandardError  = true,
//                    UseShellExecute        = false,
//                    CreateNoWindow         = true
//                };
                
//                Log.LogMessage(MessageImportance.High, spirv_crossStartInfo.FileName + " " + spirv_crossStartInfo.Arguments);

//                Errors += spirv_crossStartInfo.FileName + " " + spirv_crossStartInfo.Arguments;

//                spirv_crossProcess = new Process();
//                spirv_crossProcess.StartInfo = spirv_crossStartInfo;                
//                spirv_crossProcess.EnableRaisingEvents = true;
//                spirv_crossProcess.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
//                {
//                    if (!string.IsNullOrEmpty(e.Data))
//                    {
//                        //Log.LogMessage(MessageImportance.High, e.Data);
                        
//                        if(e.Data != sourceFilePath)
//                        {
//                            Log.LogError(e.Data);
//                        }
//                        Errors += e.Data;
//                    }
//                });
//                spirv_crossProcess.ErrorDataReceived  += new DataReceivedEventHandler((sender, e) =>
//                {
//                    if (!string.IsNullOrEmpty(e.Data))
//                    {
//                        Log.LogError(e.Data);
//                        Errors += e.Data;
//                    }
//                });
//                spirv_crossProcess.Start();                
//                spirv_crossProcess.BeginOutputReadLine();
//                spirv_crossProcess.BeginErrorReadLine();            
//                spirv_crossProcess.WaitForExit();
//                spirv_crossProcess.Close();

//            }

//            return Success;
//        }
//    }
//}

@echo off
IF NOT EXIST lib (
    mkdir lib
 
    copy packages\MySqlConnector\lib\netstandard2.0\MySqlConnector.dll lib\MySqlConnector.dll
    copy packages\System.Buffers\lib\netstandard2.0\System.Buffers.dll lib\System.Buffers.dll
    copy packages\System.Data.Common\lib\netstandard1.2\System.Data.Common.dll lib\System.Data.Common.dll
    copy packages\System.Runtime.InteropServices.RuntimeInformation\lib\netstandard1.1\System.Runtime.InteropServices.RuntimeInformation.dll lib\System.Runtime.InteropServices.RuntimeInformation.dll
    copy packages\System.Threading.Tasks.Extensions\lib\netstandard2.0\System.Threading.Tasks.Extensions.dll lib\System.Threading.Tasks.Extensions.dll
) ELSE echo "lib folder is already setup"
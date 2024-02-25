
NB: You must use grpc-tools 1.11.3 as 1.12 does not work

To regenerate client files, open powershell command prompt and run the following commands:
Run visual studio as administrator
cd GrpcClient
$env:path = $env:path + ";./node_modules/.bin/;./node_modules/grpc-tools/bin/;./node_modules/protoc-gen-grpc-web/bin/;"
.\Generate.ps1
protoc.exe -I ..\..\ --js_out=import_style=commonjs:.\ --grpc-web_out=import_style=commonjs,mode=grpcwebtext:.\ ..\..\GrpcService\Protos\datalog.proto

Then to build you source code, run the following command: (This will prompt you to install some packages on the first run)
npx webpack -o ../wwwroot/js/datalog/ ./send.js --mode development
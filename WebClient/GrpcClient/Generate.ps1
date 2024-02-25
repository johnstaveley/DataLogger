Remove-Item datalog_pb.js -force
Remove-Item datalog_grpc_web_pb.js -force
protoc.exe -I ..\..\ `
	--js_out=import_style=commonjs:.\ `
	--grpc-web_out=import_style=commonjs,mode=grpcwebtext:.\ `
	..\..\GrpcService\Protos\datalog.proto
$files = Get-ChildItem 'GrpcService/Protos/*'
Get-ChildItem $files | Move-Item -Destination { $_.Directory.Parent.Parent.FullName }
rmdir GrpcService -recurse -force
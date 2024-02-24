protoc.exe -I ..\..\ `
	--js_out=import_style=commonjs:.\ `
	--grpc-web_out=import_style=commonjs,mode=grpcwebtext:.\ `
	..\..\GrpcService\Protos\datalog.proto
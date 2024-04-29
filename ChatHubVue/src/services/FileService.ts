import { postUploadFile } from "../common/api";
interface UploadParmas{
    base64String:string,
    fileType:string,
    fileName:string,
}
interface UploadResult{
    name:string,
    url:string,
}
export class FileService {
    public async Upload(params:UploadParmas): Promise<[boolean,any]> {
        return await postUploadFile(params).then(res => {
            if(res.data.code == 1){
                return [true,JSON.parse(res.data.data)];
            }else return [false,null]
        })
    }
}
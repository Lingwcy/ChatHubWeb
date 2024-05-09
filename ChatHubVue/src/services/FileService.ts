import { postUploadFile,postUserAvatar,postUserInfo } from "../common/api";
interface UploadParmas{
    base64String:string,
    fileType:string,
    fileName:string,
}

export class FileService {
    public async Upload(params:UploadParmas): Promise<[boolean,any]> {
        return await postUploadFile(params).then(res => {
            if(res.data.code == 1){
                return [true,JSON.parse(res.data.data)];
            }else return [false,null]
        })
    }
    public async UploadAvatar(params:any): Promise<boolean> {
        return await postUserAvatar(params).then(res => {
            if(res.data.code == 1){
                return true
            }else return false
        })
    }
    public async UpdateUserInfo(params:any): Promise<boolean> {
        return await postUserInfo(params).then(res => {
            if(res.data.code == 1){
                return true
            }else return false
        })
    }
}
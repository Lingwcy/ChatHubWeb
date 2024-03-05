import { AxiosResponse } from 'axios'
import http from '../common/axiosSetting'
export const get = (url: string, config?: { params?: any, headers?: any, method?: string }):Promise<AxiosResponse<any>> =>{
    return new Promise((resolve, reject) =>{
        http.get(url,config)
        .then(res => resolve(res))
        .catch(error =>{reject(error)})
    })
}
export const post = (url:string, params:any,config?: { params?: any, headers?: any, method?: string }):Promise<AxiosResponse<any>> =>{
    return new Promise((resolve,reject) =>{
        http.post(url,params,config)
        .then(res => {resolve(res)})
        .catch(error =>{reject(error)})
    });
}
export const delete_ = (url: string, config?: { params?: any, headers?: any, method?: string }): Promise<AxiosResponse<any>> => {  
    return new Promise<AxiosResponse<any>>((resolve, reject) => {  
        http.delete(url, config)  
            .then(res => resolve(res))  
            .catch(error => reject(error));  
    });  
}  
  
export const put = (url: string, params: any): Promise<AxiosResponse<any>> => {  
    return new Promise<AxiosResponse<any>>((resolve, reject) => {  
        http.put(url, params)  
            .then(res => resolve(res))  
            .catch(error => reject(error));  
    });  
}


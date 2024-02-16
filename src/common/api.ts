import {post,get,delete_,put} from "./axios";


//Auth
export const postLogin = (params: any) =>post('/Auth/login',params);
export const postRegister = (params: any) =>post('/Auth/register',params);
export const getVerify = () =>get('/Auth/verify')


//Friends
export const getFriendRequest = (params: {userName:string}) => get('/Friends/Request',{params});
export const getFriends = (params: {userId:string}) => get('/Friends/queryAll',{params});
export const postFriendRequest = (params:any) => post('/Friends/sendRequest',params)
export const findFriend = (params:{targetName:string, userName:string}) => get('/Friends/query',{params})
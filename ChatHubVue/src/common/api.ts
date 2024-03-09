import {post,get,delete_,put} from "./axios";


//Auth
export const postLogin = (params: any) =>post('/Auth/login',params);
export const postRegister = (params: any) =>post('/Auth/register',params);
export const getVerify = () =>get('/Auth/verify')
export const postAESKey = (data:any,config: any) =>post('/Auth/AES',data,config);

//Friends
export const getFriendRequest = (params: {userName:string,xusername:string}) => get('/Friends/Request',{params});
export const getFriends = (params: {userId:string,xusername:string}) => get('/Friends/queryAll',{params});
export const postFriendRequest = (params:any) => post('/Friends/sendRequest',params);
export const findFriend = (params:{targetName:string, userName:string,xusername:string}) => get('/Friends/query',{params})
export const acceptRequest = (parmas:any) => post('/Friends/acceptRequest',parmas);
export const rejectRequest = (params:any) => delete_('/Friends/request',{params})


//Message
export const getOfflineMessage = (params:{username:string,xusername:string}) => get('Message/offline',{params})
export const postRedbob = (parmas:any) => post('/Friends/Redbob',parmas);
export const getMessageBox = (params: {username:string,xusername:string}) => get('/Friends/MessageBox',{params});
export const deleteMessageBox = (params:any) => delete_('/Message/messageBoxItem',{params})

//User
export const getUserInfo = (params:{targetname:string}) => get('/User/QueryByUserName',{params})


//Group
export const getGroupFromName = (params:{groupName:string}) => get('/Group/GetByName',{params})
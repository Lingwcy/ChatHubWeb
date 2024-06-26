import {post,get,delete_} from "./axios";


//Auth
export const postLogin = (params: any) =>post('/Auth/login',params);
export const postRegister = (params: any) =>post('/Auth/register',params);
export const getVerify = () =>get('/Auth/verify')
export const postAESKey = (data:any,config: any) =>post('/Auth/AES',data,config);

//Friends
export const getFriendRequest = (params: {userName:string,xusername:string}) => get('/Friends/Request',{params});
export const getFriends = (params: {userId:string,xusername:string}) => get('/Friends/queryAll',{params});
export const postFriendRequest = (params:any) => post('/Friends/sendRequest',params);
export const findFriendTree = (params:{userId:string,xusername:string}) => get('/Friends/querytree',{params})
export const findFriend = (params:{targetName:string, userName:string,xusername:string}) => get('/Friends/query',{params}) //不放回
export const findFriends = (params:{targetName:string, userName:string,xusername:string}) => get('/Friends/querys',{params}) //放回数据
export const acceptRequest = (parmas:any) => post('/Friends/acceptRequest',parmas);
export const rejectRequest = (params:any) => delete_('/Friends/request',{params})
export const deleteFriend = (params:any) => delete_('/Friends/delete',{params})


//Message
export const getOfflineMessage = (params:{username:string,xusername:string}) => get('Message/offline',{params})
export const postRedbob = (parmas:any) => post('/Friends/Redbob',parmas);
export const getMessageBox = (params: {username:string,xusername:string}) => get('/Friends/MessageBox',{params});
export const deleteMessageBox = (params:any) => delete_('/Message/messageBoxItem',{params})

//User
export const getUserInfo = (params:{targetname:string}) => get('admin/User/QueryByUserName',{params})


//Group
export const getGroupFromName = (params:{groupName:string}) => get('/Group/GetByName',{params})
export const getGroupList = (params:{userId:number,xusername:string}) => get('/Group/Mygroups',{params})
export const getGroupMembers = (params:{groupId:number,xusername:string}) => get('/Group/Members',{params})
export const postGroupRequest = (params:any) => post('/Group/SendRequest',params);
export const getGroupRequest = (params:{userId:number,xusername:string}) => get('/Group/Requests',{params});
export const postAcceptGroupRequest = (params:any) => post('/Group/AcceptRequest',params);
export const deleteRejectGroupRequest = (params:any) => delete_('/Group/RejectRequest',{params})
export const postCreateGroup = (params:any) => post('/Group/Create',params);
export const postChangeGroupName = (params:any) => post('/Group/ChangeGroupName',params);
export const postChangeGroupNotice = (params:any) => post('/Group/ChangeGroupNotice',params);
export const postExitGroup = (params:any) => post('/Group/ExitGroup',params);
export const postDismissGroup = (params:any) => post('/Group/DismissGroup',params);


//File
export const postUploadFile = (params:any) => post('/File/Upload',params);
export const postUserAvatar = (params:any) => post('/File/UserAvatar',params);
export const postUserInfo = (params:any) => post('/File/UserInfo',params);
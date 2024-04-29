interface IUserProfile {  
    id: number;  
    Username: string;  
    HeaderImg: string;  
    Email: string;  
    City: string;  
    Sex: string;  
    Age: string;  
    Job: string;  
    Phone: string;  
    NickName: string;  
    Birth: string;  
    Desc: string;
    status:number
}

interface FriendTree {  
    OwnerId?: number; // 使用number类型来对应C#中的long，并且使用?表示可选  
    Units: FriendTreeUnit[]; // 使用数组来对应List<T>  
}  
  
interface FriendTreeUnit {  
    id: number; // 假设ID是必需的，因此不使用?  
    UnitName?: string; // UnitName可能为空，使用?表示可选  
    Children?: IUserProfile[]; // 假设UserList可能为空，使用数组来对应List<T>  
}  

import { SlateElement } from '@wangeditor/editor'

export type ImageElement = SlateElement & {
    src: string
    alt: string
    url: string
    href: string
}
export type InsertFnType = (url: string, alt: string, href: string) => void
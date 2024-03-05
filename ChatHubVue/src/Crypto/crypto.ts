import CryptoJS from 'crypto-js'

export interface CrypotoType {
    encrypt: any
    decrypt: any
}
 class Crypoto implements CrypotoType {
    public key = CryptoJS.enc.Utf8.parse('123456789asdfghj');
    public iv = CryptoJS.enc.Utf8.parse('123456789asdfghj');
    public gkey = ''

    constructor(){
        this.generateRandomKey();
    }

    generateRandomKey(length = 16) {
        const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
        let result = '';
        const charactersLength = characters.length;

        for (let i = 0; i < length; i++) {
            result += characters.charAt(Math.floor(Math.random() * charactersLength));
        }

        this.key = CryptoJS.enc.Utf8.parse(result);
        this.iv = this.key;
        this.gkey = result;
    }

    encryptByAES(input: string): string {  
        if (input == null || input.trim() === '') {  
            return input;  
        }   
      
        // 使用AES加密，CBC模式和PKCS7填充  
        const encrypted = CryptoJS.AES.encrypt(input, this.key, {  
            iv: this.iv,  
            mode: CryptoJS.mode.CBC,  
            padding: CryptoJS.pad.Pkcs7  
        });  
      
        // 返回Base64编码的加密字符串  
        return encrypted.ciphertext.toString(CryptoJS.enc.Base64)  
    }


    /* 加密 */
    encrypt(word: any): string {
        const srcs = CryptoJS.enc.Utf8.parse(word)
        const encrypted = CryptoJS.AES.encrypt(srcs, this.key, { iv: this.iv, mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7 })
        return encrypted.ciphertext.toString()
    }

    /* 解密 */
    decrypt(word: any): string {
        const encryptedHexStr = CryptoJS.enc.Hex.parse(word)
        const srcs = CryptoJS.enc.Base64.stringify(encryptedHexStr)
        const decrypt = CryptoJS.AES.decrypt(srcs, this.key, { iv: this.iv, mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7 })
        const decryptedStr = decrypt.toString(CryptoJS.enc.Utf8)
        return decryptedStr.toString()
    }
}

export let crypto = new Crypoto(); 
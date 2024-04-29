<template>
    <div class="wangeditor">
        <Toolbar :editor="editorRef" :defaultConfig="toolbarConfig" :mode="mode" />
        <Editor class="editor-content" v-model="valueHtml" :defaultConfig="editorConfig" :mode="mode"
            @onCreated="handleCreated" @keyup.enter="handleEnter" />
        <el-tooltip effect="dark" content="按Enter发送消息, Ctrl+Enter换行" placement="right-end">
            <el-button class="btn-send" @click="sendMessage"> 发送 </el-button>
        </el-tooltip>
    </div>
</template>


<script setup lang="ts">
import { shallowRef, ref, onBeforeUnmount, reactive } from 'vue';
import '@wangeditor/editor/dist/css/style.css' // 引入 css
import { IToolbarConfig, IEditorConfig, DomEditor, } from '@wangeditor/editor'
import { Editor, Toolbar } from '@wangeditor/editor-for-vue'
import { InsertFnType } from '../../models/data/Entity';
import { UseServiceStore } from '../../store';
import { createFileService } from '../../services/ServicesCollector';
import { extractImageInfo } from '../MainContent/HubContent/utils';
const service = UseServiceStore()
const editorRef = shallowRef()
createFileService()
const mode = 'simple'
const valueHtml = ref('')

const sendMessage = (text: string, images: any) => {
    if (editorRef.value.getText().toString().trim() != '') service.ChatHub?.SendMessageToServer(text);
    if (images.length > 0) {
        images.forEach((element: any) => {
            service.ChatHub?.SendImageToServer(element);
        });
    }
    console.log(text)
    clearInputInfo()
}



const toolbarConfig: Partial<IToolbarConfig> = {
}
toolbarConfig.toolbarKeys = [
    "emotion",
    {
        key: "group-image",
        iconSvg: '<svg viewBox="0 0 1024 1024"><path d="M959.877 128l0.123 0.123v767.775l-0.123 0.122H64.102l-0.122-0.122V128.123l0.122-0.123h895.775zM960 64H64C28.795 64 0 92.795 0 128v768c0 35.205 28.795 64 64 64h896c35.205 0 64-28.795 64-64V128c0-35.205-28.795-64-64-64zM832 288.01c0 53.023-42.988 96.01-96.01 96.01s-96.01-42.987-96.01-96.01S682.967 192 735.99 192 832 234.988 832 288.01zM896 832H128V704l224.01-384 256 320h64l224.01-192z"></path></svg>', // 可选
        menuKeys: ['insertImage', 'uploadImage'],
        title: '图片',
    },
    'undo',
    'redo',


]
const editorConfig: Partial<IEditorConfig> = {
    MENU_CONF: {},
    placeholder: '按Enter发送消息, Ctrl+Enter换行',
    maxLength: 1000,
}

editorConfig.MENU_CONF['uploadImage'] = {
    // form-data fieldName ，默认值 'wangeditor-uploaded-image'
    fieldName: 'your-custom-name',
    server: '/api/upload',
    // 单个文件的最大体积限制，默认为 2M
    maxFileSize: 1 * 1024 * 1024 * 100, // 100M

    // 最多可上传几个文件，默认为 100
    maxNumberOfFiles: 10,

    // 选择文件时的类型限制，默认为 ['image/*'] 。如不想限制，则设置为 []
    allowedFileTypes: ['image/*'],

    // 自定义上传
    async customUpload(file: File, insertFn: InsertFnType) {  // TS 语法
        try {
            const fileType = file.type.split('/')[1]
            // 读取文件的二进制数据  
            const reader = file.stream().getReader();
            const { value, done } = await reader.read();
            if (!done) {
                const buffer = new Uint8Array(value).buffer; // 转换为 ArrayBuffer
                const binary = new Uint8Array(buffer); // 转换为二进制数组
                let base64 = "";
                for (let i = 0; i < binary.length; i++) {
                    base64 += String.fromCharCode(binary[i]);
                }
                base64 = btoa(base64); // 将二进制数据转换为 base64
                const payload = {
                    base64String: base64,
                    fileType: fileType,
                    fileName: file.name,
                }
                service.File?.Upload(payload).then((res) => {
                    const [flag, data] = res;
                    if (flag) {
                        insertFn('https://localhost:5001/File/GetImage?filename=' + data.name, '', 'https://localhost:5001/File/GetImage?filename=' + data.name)
                    }
                })
                //insertFn('https://localhost:5001/File/GetImage?filename=test.png', '', 'http://www.w3.org/2000/svg')
            } else {
                // 处理文件读取完成的情况
            }
        } catch (error) {
            console.log(error)
        }
    }
}

onBeforeUnmount(() => {
    const editor = editorRef.value
    if (editor == null) return
    editor.destroy()
})
const handleCreated = (editor: any) => {
    editorRef.value = editor // 记录 editor 实例，重要！
}
const sendMsgBefore = (editor = editorRef.value) => {
    const text = editor.getText(); // 纯文本内容
    const { images } = extractImageInfo(editor);
    return {
        text,
        images,
    }
};

// 回车
const handleEnter = (event: any) => {
    if (event?.ctrlKey) {
        editorRef.value.insertBreak()
    } else {
        const { text, images } = sendMsgBefore();
        sendMessage(text, images);
    }

};
// 清空输入框
const clearInputInfo = () => {
    const editor = editorRef.value;
    editor && editor.clear();
};




</script>

<style>
:root,
:host {
    --w-e-textarea-bg-color: #f4f4f5;
    --w-e-textarea-color: #000000;
    --w-e-textarea-border-color: #ffffff;
    --w-e-textarea-slight-border-color: #e9e9eb;
    --w-e-textarea-slight-color: #a4a4a5;
    --w-e-textarea-slight-bg-color: #e9e9eb;
    --w-e-textarea-selected-border-color: #e9e9eb;
    --w-e-textarea-handler-bg-color: #4290f7;

    --w-e-toolbar-color: #595959;
    --w-e-toolbar-bg-color: #f4f4f5;
    --w-e-toolbar-active-color: #110404;
    --w-e-toolbar-active-bg-color: #f1f1f1;
    --w-e-toolbar-disabled-color: #999;
    --w-e-toolbar-border-color: #e9e9eb;

    --w-e-modal-button-bg-color: #fafafa;
    --w-e-modal-button-border-color: #d9d9d9;
}

.btn-send {
    bottom: 8px;
    right: 16px;
    position: sticky;
    margin-left: 89%;
}

.wangeditor {
    word-break: break-all;
    height: 236px;

    .editor-content {
        height: calc(100% - 40px) !important;
        overflow-y: hidden;

        :deep(.w-e-text-container p) {
            margin: 0;
        }

        :deep(.w-e-image-dragger) {
            display: none;
        }

        :deep(.w-e-text-placeholder) {
            font-style: normal;
            font-size: 15px;
            top: 5px;
        }

        :deep(.w-e-selected-image-container) {
            overflow: visible;
        }

        :deep(.w-e-scroll) {
            background-color: saddlebrown;
        }
    }

    .btn-send {}
}
</style>
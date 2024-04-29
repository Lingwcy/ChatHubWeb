<template>
  <el-upload action="#" list-type="picture-card" :auto-upload="false" :before-upload="handleBeforeUpload"
    ref="uploadRef">

    <el-icon>
      <Plus />
    </el-icon>

    <template #file="{ file }">
      <div>
        <img class="el-upload-list__item-thumbnail" :src="file.url" alt="" />
        <span class="el-upload-list__item-actions">
          <span class="el-upload-list__item-preview" @click="handlePictureCardPreview(file)">
            <el-icon><zoom-in /></el-icon>
          </span>
          <span v-if="!disabled" class="el-upload-list__item-delete" @click="handleDownload(file)">
            <el-icon>
              <Download />
            </el-icon>
          </span>
          <span v-if="!disabled" class="el-upload-list__item-delete" @click="handleRemove(file)">
            <el-icon>
              <Delete />
            </el-icon>
          </span>
        </span>
      </div>
    </template>
  </el-upload>

  <el-dialog v-model="dialogVisible">
    <img w-full :src="dialogImageUrl" alt="Preview Image" />
  </el-dialog>
</template>
<script lang="ts" setup>
import { ref } from 'vue'
import { Delete, Download, Plus, ZoomIn } from '@element-plus/icons-vue'
import { UseServiceStore } from '../../../../store';
import type { UploadFile } from 'element-plus'
import { uploadRef } from './PicUploadHook';

const dialogImageUrl = ref('')
const service = UseServiceStore()
const dialogVisible = ref(false)
const disabled = ref(false)

const handleBeforeUpload = async (file: File) => {
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
      await service.ChatHub?.SendImgTest(base64,fileType);
    } else {
      // 处理文件读取完成的情况
    }
  } catch (error) {
    console.log(error)
  }
};







const handlePictureCardPreview = (file: UploadFile) => {
  dialogImageUrl.value = file.url!
  dialogVisible.value = true
}

const handleDownload = (file: UploadFile) => {
  console.log(file)
}
</script>

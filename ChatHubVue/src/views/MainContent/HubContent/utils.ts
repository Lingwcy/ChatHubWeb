/**
 * 提取图片信息
 */
export const extractImageInfo = (editor:any) => {
    let images = null;
    const image = editor.getElemsByType("image");
    // 过滤表情包消息
    images = image.filter((item: { class: string; }) => item.class !== "EmoticonPack");
    return { images };
  };
  function isVideoFile(fileName: string) {
    const video = ["mp4", "wmv", "webm"];
    const name = fileName.toLowerCase();
    const regex = new RegExp(`(${video.join("|")})$`, "i");
    return regex.test(name);
  }
  /**
   * 提取文件信息
   */
  export const extractFilesInfo = (editor: { getElemsByType: (arg0: string) => any; }) => {
    const file: any[] = [];
    const files = editor.getElemsByType("attachment");
    files.map((t: { fileName: any; }) => !isVideoFile(getFileType(t.fileName)) && file.push(t));
    return { files: file };
  };
  /**
   * 提取视频信息
   */
  export const extractVideoInfo = (editor: { getElemsByType: (arg0: string) => any; }) => {
    const video: any[] = [];
    const files = editor.getElemsByType("attachment");
    files.map((t: { fileName: any; }) => isVideoFile(getFileType(t.fileName)) && video.push(t));
    return { video };
  };
  


function getFileType(fileName: any): string {
  throw new Error("Function not implemented.");
}
  
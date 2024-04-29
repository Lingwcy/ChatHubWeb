/**
 * 提取图片信息
 */
export const extractImageInfo = (editor:any) => {
    let images = null;
    const image = editor.getElemsByType("image");
    // 过滤表情包消息
    images = image.filter((item) => item.class !== "EmoticonPack");
    return { images };
  };
  function isVideoFile(fileName) {
    const video = ["mp4", "wmv", "webm"];
    const name = fileName.toLowerCase();
    const regex = new RegExp(`(${video.join("|")})$`, "i");
    return regex.test(name);
  }
  /**
   * 提取文件信息
   */
  export const extractFilesInfo = (editor) => {
    const file = [];
    const files = editor.getElemsByType("attachment");
    files.map((t) => !isVideoFile(getFileType(t.fileName)) && file.push(t));
    return { files: file };
  };
  /**
   * 提取视频信息
   */
  export const extractVideoInfo = (editor) => {
    const video = [];
    const files = editor.getElemsByType("attachment");
    files.map((t) => isVideoFile(getFileType(t.fileName)) && video.push(t));
    return { video };
  };
  
  /**
   * 从编辑器中提取 Ait 信息
   * @param {Object} editor - 编辑器对象，包含编辑器的内容和方法
   * @returns {Object} - 包含提及字符串和提及的 id 列表的对象
   */
  export const extractAitInfo = (editor) => {
    let aitStr = "";
    let aitlist = [];
    const html = editor.getHtml();
    const mention = editor.getElemsByType("mention");
    if (mention.length) {
      // 清除文件消息包含的字符串
      const fileRegex = /<span\s+data-w-e-type="attachment"[^>]*>(.*?)<\/span>/g;
      let str = html.replace(fileRegex, "");
      // 清除 HTML 标签和 &nbsp
      aitStr = str.replace(/<[^>]+>/g, "").replace(/&nbsp;/gi, "");
      mention.forEach((t) => aitlist.push(t.info.id));
      aitlist = Array.from(new Set(aitlist));
    }
    return { aitStr, aitlist };
  };
  
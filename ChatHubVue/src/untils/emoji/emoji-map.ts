import { emojiMap as qqEmojiMap, emojiName as qqEmojiName } from "./emoji-map-qq";
import { emojiMap as douyinEmojiMap, emojiName as douyinEmojiName } from "./emoji-map-douyin";

export const emojiUrl = "https://www.emojiall.com/img/platform/qq/";
export const localemojiUrl = "@/assets/emoji/";

export const emojiMap = {
  ...qqEmojiMap,
  ...douyinEmojiMap,
};

export const emojiName = [
  ...qqEmojiName,
  ...douyinEmojiName,
];

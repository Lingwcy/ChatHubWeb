import axios from "axios";
import {UseUserInformationStore} from '../store/index'
const userInfo = UseUserInformationStore()
axios.defaults.baseURL = 'https://localhost:5001/';//基础Url

export default axios
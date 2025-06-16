import * as playerApi from './playerApi'
import * as roomApi from './roomApi'
import * as dailiesApi from './dailiesApi'

const api = {
  ...playerApi,
  ...roomApi,
  ...dailiesApi
}

export default api

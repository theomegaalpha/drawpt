import * as playerApi from './playerApi'
import * as roomApi from './roomApi'
import * as dailiesApi from './dailiesApi'
import * as musicApi from './musicApi'

const api = {
  ...playerApi,
  ...roomApi,
  ...dailiesApi,
  ...musicApi
}

export default api

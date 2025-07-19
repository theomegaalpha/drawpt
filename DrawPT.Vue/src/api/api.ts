import * as playerApi from './playerApi'
import * as roomApi from './roomApi'
import * as dailiesApi from './dailiesApi'
import * as musicApi from './musicApi'
import * as miscApi from './miscApi'

const api = {
  ...playerApi,
  ...roomApi,
  ...dailiesApi,
  ...musicApi,
  ...miscApi
}

export default api

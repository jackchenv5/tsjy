import { useLocaleStore } from '#/common/stores/localeStore'
import { facilityStatusLang } from './langs'

import '#/common/styles/custom/index.css'
import '#/common/styles/element/index.css'
import '#/common/styles/custom/nprogress.css'

export default {
  install: () => {
    const localeStore = useLocaleStore()

    localeStore.addLang(facilityStatusLang)
  },
}

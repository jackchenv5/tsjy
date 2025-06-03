import { useLocaleStore } from '#common/stores/localeStore'
import { facilityMaintainLang } from './langs'

import '#common/styles/custom/index.css'
import '#common/styles/element/index.css'
import '#common/styles/custom/nprogress.css'

export default {
  install: () => {
    const localeStore = useLocaleStore()

    localeStore.addLang(facilityMaintainLang)
  },
}

import { useLocaleStore } from './stores/localeStore'
import { commonLang } from './langs'

import './styles/custom/index.css'
import './styles/element/index.css'
import './styles/custom/nprogress.css'

export default {
  install: () => {
    const localeStore = useLocaleStore()

    localeStore.addLang(commonLang)
  },
}

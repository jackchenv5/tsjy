import { stoppedDataLang } from './stoppedData'
import { statusBindingLang } from './statusBinding'
import { statusSummaryLang } from './statusSummary'

export const facilityStatusLang = {
  en: {
    el: {
      facilityStatus: {
        ...stoppedDataLang.en.el,
        ...statusBindingLang.en.el,
        ...statusSummaryLang.en.el,
      },
    },
  },
  zhCn: {
    el: {
      facilityStatus: {
        ...stoppedDataLang.zhCn.el,
        ...statusBindingLang.zhCn.el,
        ...statusSummaryLang.zhCn.el,
      },
    },
  },
}

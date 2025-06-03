import { partDetailLang } from './partDetail'
import { maintainPlanLang } from './maintainPlan'
import { maintainPlansLang } from './maintainPlans'
import { maintainSettingLang } from './maintainSetting'
import { maintainHistoryLang } from './maintainHistory'

export const facilityMaintainLang = {
  en: {
    el: {
      facilityMaintain: {
        ...partDetailLang.en.el,
        ...maintainPlanLang.en.el,
        ...maintainPlansLang.en.el,
        ...maintainSettingLang.en.el,
        ...maintainHistoryLang.en.el,
      },
    },
  },
  zhCn: {
    el: {
      facilityMaintain: {
        ...partDetailLang.zhCn.el,
        ...maintainPlanLang.zhCn.el,
        ...maintainPlansLang.zhCn.el,
        ...maintainSettingLang.zhCn.el,
        ...maintainHistoryLang.zhCn.el,
      },
    },
  },
}

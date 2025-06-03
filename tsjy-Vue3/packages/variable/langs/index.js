import { connectorStatusLang } from './connectorStatus'
import { variableMonitorLang } from './variableMonitor'
import { variableArchiveLang } from './variableArchive'

export const variableLang = {
  en: {
    el: {
      variable: {
        selectVariable: 'Select Variable',
        connectorInstance: 'Connector Instance',
        connectionName: 'Connection Name',
        dataPointName: 'Data Point Name',
        name: 'Name',
        select: 'Select',

        unbinding: 'Unbinding',
        unbindingConfirm:
          'After unbinding, the corresponding data cannot be processed. Are you sure you want to unbind?',

        ...connectorStatusLang.en.el,
        ...variableMonitorLang.en.el,
        ...variableArchiveLang.en.el,
      },
    },
  },
  zhCn: {
    el: {
      variable: {
        selectVariable: '选择变量',
        connectorInstance: '连接器实例',
        connectionName: '连接名称',
        dataPointName: '数据点名称',
        name: '名称',
        select: '选择',

        unbinding: '解绑',
        unbindingConfirm: '解绑后，无法处理相应的数据，确定要解绑吗？',

        ...connectorStatusLang.zhCn.el,
        ...variableMonitorLang.zhCn.el,
        ...variableArchiveLang.zhCn.el,
      },
    },
  },
}

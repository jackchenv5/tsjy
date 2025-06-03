<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import AddAlarmDefinition from '@/components/AddAlarmDefinition.vue'
import { h, onMounted, ref } from 'vue'
import {
  addAlarmDefinitionsApi,
  getAlarmDefinitionsApi,
} from '@/apis/sawingMachineAlarm.js'
import UpdateAlarmDefinition from '@/components/UpdateAlarmDefinition.vue'
import DeleteAlarmDefinition from '@/components/DeleteAlarmDefinition.vue'
import * as xlsx from 'xlsx'
import { dayjs, ElLoading, ElMessage, ElMessageBox } from 'element-plus'
import { useFacilities } from '#/facility/composables/facilities.js'
import { useGetDefinitions } from '#/variable/composables/getDefinitions.js'

// region 组件
onMounted(async () => {
  await getAlarmDefinitions()
})
// endregion

// region 添加定义
const showAddAlarmDefinition = ref(false)
const handleAddDefinition = () => {
  showAddAlarmDefinition.value = true
}
const handleAddDefinitionFinished = async () => {
  await getAlarmDefinitions()
}
// endregion

// region 获取报警定义
const alarmDefinitions = ref({
  total: 0,
  items: [],
})
const loadingGetAlarmDefinitions = ref(false)

const getAlarmDefinitions = async () => {
  if (loadingGetAlarmDefinitions.value) {
    return
  }
  loadingGetAlarmDefinitions.value = true
  try {
    const { data } = await getAlarmDefinitionsApi({
      pageIndex: pageIndex.value,
      pageSize: pageSize.value,
    })
    alarmDefinitions.value = data
  } catch (e) {
    console.error(e)
  } finally {
    loadingGetAlarmDefinitions.value = false
  }
}
const getTriggerString = (row) => {
  switch (row.triggerType) {
    case 0:
      return `> ${row.triggerValue}`
    case 1:
      return `< ${row.triggerValue}`
    case 2:
      return `= ${row.triggerValue}`
    case 3:
      return `≠ ${row.triggerValue}`
    case 4:
      return `≥ ${row.triggerValue}`
    case 5:
      return `≤ ${row.triggerValue}`
  }
}
// endregion

// region 分页
const pageIndex = ref(1)
const pageSize = ref(20)
const onUpdateCurrentPage = async () => {
  await getAlarmDefinitions()
}
const onUpdatePageSize = async () => {
  await getAlarmDefinitions()
}
// endregion

// region 更新
const currentAlarmDefinition = ref()
const showUpdateAlarmDefinition = ref(false)
const handleShowUpdateAlarmDefinition = (row) => {
  currentAlarmDefinition.value = JSON.parse(JSON.stringify(row))
  showUpdateAlarmDefinition.value = true
}
const handleUpdateAlarmDefinitionFinished = async () => {
  await getAlarmDefinitions()
}
// endregion

// region 删除
const currentDeleteAlarmDefinition = ref()
const showDeleteAlarmDefinition = ref(false)
const handleShowDeleteAlarmDefinition = (row) => {
  currentDeleteAlarmDefinition.value = JSON.parse(JSON.stringify(row))
  showDeleteAlarmDefinition.value = true
}
const handleDeleteAlarmDefinitionFinished = async () => {
  await getAlarmDefinitions()
}
// endregion

// region export
const getTriggerTypeStr = (type) => {
  switch (type) {
    case 0:
      return `>`
    case 1:
      return `<`
    case 2:
      return `=`
    case 3:
      return `≠`
    case 4:
      return `≥`
    case 5:
      return `≤`
  }
}
const exportAlarmDefinitions = async () => {
  const data = alarmDefinitions.value.items.map((item) => {
    return {
      facilityName: item['facility']['name'],
      connectorInstance: item['connectorInstance'],
      connectionName: item['connectionName'],
      dataPoint: item['dataPoint'],
      name: item['name'],
      message: item['message'],
      triggerType: getTriggerTypeStr(item['triggerType']),
      dataType: item['dataType'],
      triggerValue: item['triggerValue'],
    }
  })
  const header = {
    facilityName: '设备名称',
    connectorInstance: '连接器实例',
    connectionName: '连接名称',
    dataPoint: '数据点名称',
    name: '变量名称',
    message: '报警消息',
    triggerType: '触发条件',
    dataType: '数据类型',
    triggerValue: '触发值',
  }
  const wb = xlsx.utils.book_new()
  const ws = xlsx.utils.json_to_sheet([header, ...data], { skipHeader: true })
  xlsx.utils.book_append_sheet(wb, ws, 'Sheet1')

  xlsx.writeFile(wb, `报警定义-${dayjs().format('YYYYMMDDhhmmss')}.xlsx`)
}

const uploadRef = ref()
const onUploadExceed = (files) => {
  uploadRef.value.clearFiles()
  uploadRef.value.handleStart(files[0])
}
const { facilities } = useFacilities()
const parseTriggerType = (typeStr) => {
  switch (typeStr) {
    case '>':
      return 0
    case '<':
      return 1
    case '=':
      return 2
    case '≠':
    case '!=':
      return 3
    case '≥':
    case '>=':
      return 4
    case '≤':
    case '<=':
      return 5
    default:
      return -1
  }
}

const { data: definitions, getDefinitions, nameFilter } = useGetDefinitions()
const onUploadChange = (file) => {
  const reader = new FileReader()
  reader.onload = async () => {
    if (reader.result) {
      await importDefinitions(reader.result)
    }
  }
  reader.readAsArrayBuffer(file.raw)
}

const importDefinitions = async (rawData) => {
  const loading = ElLoading.service({
    lock: true,
    text: '正在处理，请稍后...',
  })
  const wb = xlsx.read(rawData, { type: 'binary' })
  const ws = wb.Sheets[wb.SheetNames[0]]
  let data = xlsx.utils
    .sheet_to_json(ws, {
      header: [
        'facilityName',
        'connectorInstance',
        'connectionName',
        'dataPoint',
        'name',
        'message',
        'triggerType',
        'dataType',
        'triggerValue',
      ],
    })
    .slice(1)
  let errors = []

  let newDefinitions = []
  // 检查每个定义
  for (let i = 0; i < data.length; i++) {
    const item = data[i]
    nameFilter.value = item['name']
    await getDefinitions()
    const facility = facilities.value.find(
      (f) => f.name === item['facilityName'],
    )
    let err = ''
    // facilityId
    if (!facility) {
      err += `[设备名称 ${item['facilityName']} 错误]`
    }
    // triggerType
    if (parseTriggerType(item['triggerType']) === -1) {
      err += ` [触发条件 ${item['triggerType']} 错误]`
    }
    // dataType
    const dataTypes = ['int', 'float']
    if (dataTypes.findIndex((t) => t === item['dataType']) < 0) {
      err += ` [数据类型 ${item['dataType']} 错误]`
    }
    // triggerValue
    if (isNaN(Number(item['triggerValue']))) {
      err += ` [触发值 ${item['triggerValue']} 错误]`
    }
    // message
    if (!item['message']) {
      err += ` [报警消息 ${item['dataType']} 不能为空]`
    }
    // variable
    const index = definitions.value.items.findIndex(
      (v) =>
        v['connectorInstance'] === item['connectorInstance'] &&
        v['connectionName'] === item['connectionName'] &&
        v['dataPointName'] === item['dataPoint'] &&
        v['name'] === item['name'],
    )
    if (index < 0) {
      err += ` [变量 ${item['name']} 错误]`
    }

    if (err) {
      errors.push(`[行 ${i + 1}] ${err}`)
    } else {
      newDefinitions.push({
        connectorInstance: item['connectorInstance'],
        connectionName: item['connectionName'],
        dataPoint: item['dataPoint'],
        name: item['name'],
        facilityId: facility['id'],//facility['id'],
        message: item['message'],
        triggerType: parseTriggerType(item['triggerType']),
        dataType: item['dataType'],
        triggerValue: item['triggerValue'],
      })
    }
  }
  
  if (errors.length === 0) {
    try {
      await addAlarmDefinitionsApi(newDefinitions)
      ElMessage({
        type: 'success',
        message: '导入成功',
      })
      await getAlarmDefinitions()
    } catch (e) {
      console.error(e)
      ElMessage({
        type: 'success',
        message: '导入失败',
      })
    }
  } else {
    ElMessageBox({
      type: 'error',
      title: '数据错误',
      message: () => errors.map((err) => h('p', err)),
    })
  }
  loading.close()
}
// endregion
</script>

<template>
  <ContentLayout>
    <template #header>报警定义</template>
    <template #default>
      <div class="content-container">
        <div class="header-container">
          <ElButton type="primary" plain @click="handleAddDefinition">
            添加定义
          </ElButton>
          <ElDivider direction="vertical"></ElDivider>
          <ElUpload
            :limit="1"
            :auto-upload="false"
            :accept="'.xlsx'"
            :action="''"
            :show-file-list="false"
            :on-exceed="onUploadExceed"
            ref="uploadRef"
            :on-change="onUploadChange"
          >
            <ElButton type="primary" text>导入</ElButton>
          </ElUpload>
          <ElButton type="primary" text @click="exportAlarmDefinitions">
            导出
          </ElButton>
        </div>
        <ElTable
          class="table"
          stripe
          show-overflow-tooltip
          :data="alarmDefinitions.items"
        >
          <ElTableColumn label="设备名称" prop="facility.name" width="120">
          </ElTableColumn>
          <ElTableColumn
            label="连接器实例"
            prop="connectorInstance"
            width="170"
          >
          </ElTableColumn>
          <ElTableColumn label="连接名称" prop="connectionName" width="160">
          </ElTableColumn>
          <ElTableColumn label="数据点名称" prop="dataPoint" width="150">
          </ElTableColumn>
          <ElTableColumn label="变量名称" prop="name" min-width="240">
          </ElTableColumn>
          <ElTableColumn label="触发条件" prop="" width="160">
            <template #default="{ row }">
              {{ getTriggerString(row) }}
            </template>
          </ElTableColumn>
          <ElTableColumn label="报警消息" prop="message" min-width="240">
          </ElTableColumn>
          <ElTableColumn width="140">
            <template #default="{ row }">
              <ElButtonGroup size="small">
                <ElButton
                  type="primary"
                  plain
                  @click="handleShowUpdateAlarmDefinition(row)"
                >
                  编辑
                </ElButton>
                <ElButton
                  type="danger"
                  plain
                  @click="handleShowDeleteAlarmDefinition(row)"
                >
                  删除
                </ElButton>
              </ElButtonGroup>
            </template>
          </ElTableColumn>
        </ElTable>
        <ElPagination
          layout="prev, pager, next, sizes, total"
          :total="alarmDefinitions.total"
          :default-page-size="20"
          :pager-count="5"
          v-model:current-page="pageIndex"
          @update:current-page="onUpdateCurrentPage"
          v-model:page-size="pageSize"
          :page-sizes="[20, 50, 100]"
          @update:page-size="onUpdatePageSize"
        >
        </ElPagination>
      </div>
    </template>
  </ContentLayout>
  <AddAlarmDefinition
    v-model="showAddAlarmDefinition"
    @finished="handleAddDefinitionFinished"
  >
  </AddAlarmDefinition>
  <UpdateAlarmDefinition
    v-model="showUpdateAlarmDefinition"
    :alarm-definition="currentAlarmDefinition"
    @finished="handleUpdateAlarmDefinitionFinished"
  >
  </UpdateAlarmDefinition>
  <DeleteAlarmDefinition
    v-model="showDeleteAlarmDefinition"
    :alarm-definition="currentDeleteAlarmDefinition"
    @finished="handleDeleteAlarmDefinitionFinished"
  >
  </DeleteAlarmDefinition>
</template>

<style scoped lang="scss">
.content-container {
  flex: 1;
  display: flex;
  flex-flow: column;
  grid-gap: 1rem;

  > .header-container {
    padding-bottom: 1rem;
    display: flex;
    flex-flow: row;
    align-items: center;
  }

  > .table {
    flex: 1;
  }
}
</style>

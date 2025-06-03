<script setup>
import { inject, nextTick, onMounted, ref, watch } from 'vue'
import { getAlarmHistoryApi } from '@/apis/sawingMachineAlarm.js'
import { dayjs } from 'element-plus'

// region 组件
const startTime = inject('startTime')
const endTime = inject('endTime')
onMounted(async () => {
  await nextTick(async () => {
    await getHistoryData()
  })
})
const model = defineModel()
watch(model, async (value) => {
  if (value) {
    await getHistoryData()
    model.value = false
  }
})
// endregion

// region 报警历史数据
const historyData = ref({
  total: 0,
  items: [],
})
const loadingGetHistoryData = ref(false)
const getHistoryData = async () => {
  if (loadingGetHistoryData.value) {
    return
  }
  loadingGetHistoryData.value = true
  try {
    const param = {
      pageIndex: pageIndex.value,
      pageSize: pageSize.value,
      startTime: dayjs(startTime.value).unix(),
      endTime: dayjs(endTime.value).unix(),
    }
    const { data } = await getAlarmHistoryApi(param)
    historyData.value = data
  } catch (e) {
    console.error(e)
  } finally {
    loadingGetHistoryData.value = false
  }
}
// endregion

// region 分页
const pageIndex = ref(1)
const pageSize = ref(20)
const onUpdateCurrentPage = async () => {
  await getHistoryData()
}
const onUpdatePageSize = async () => {
  await getHistoryData()
}
// endregion
</script>

<template>
  <div class="container">
    <div class="header">报警记录</div>
    <div class="content">
      <ElTable
        class="table"
        stripe
        show-overflow-tooltip
        :data="historyData.items"
      >
        <ElTableColumn label="设备名称" prop="facility.name" width="180">
        </ElTableColumn>
        <ElTableColumn label="开始时间" prop="startTime" width="180">
          <template #default="{ row }">
            <ElTag type="warning" size="small">
              {{ dayjs.unix(row['startTime']).format('YYYY-MM-DD HH:mm:ss') }}
            </ElTag>
          </template>
        </ElTableColumn>
        <ElTableColumn label="结束时间" prop="endTime" width="180">
          <template #default="{ row }">
            <ElTag type="success" size="small">
              {{ dayjs.unix(row['endTime']).format('YYYY-MM-DD HH:mm:ss') }}
            </ElTag>
          </template>
        </ElTableColumn>
        <ElTableColumn label="报警消息" prop="message" min-width="180">
        </ElTableColumn>
      </ElTable>
      <ElPagination
        layout="prev, pager, next, sizes, total"
        :total="historyData.total"
        :default-page-size="20"
        :pager-count="5"
        v-model:current-page="pageIndex"
        @update:current-page="onUpdateCurrentPage"
        v-model:page-size="pageSize"
        :page-sizes="[20, 50, 100]"
        @update:page-size="onUpdatePageSize"
        :disabled="loadingGetHistoryData"
        style="margin-bottom: 1rem"
      >
      </ElPagination>
    </div>
  </div>
</template>

<style scoped lang="scss">
.container {
  border: 1px solid var(--el-border-color);
  border-radius: var(--el-border-radius-base);
  flex: 1;
  display: flex;
  flex-flow: column;
  overflow: auto;

  > .header {
    display: flex;
    flex-flow: row;
    grid-gap: 1rem;
    align-items: center;
    background-color: var(--el-color-info-light-9);
    padding: 0.5rem 1rem;
    border-radius: var(--el-border-radius-base) var(--el-border-radius-base) 0 0;
    border-bottom: 1px solid var(--el-border-color);
  }

  > .content {
    flex: 1;
    display: flex;
    flex-flow: column;
    grid-gap: 1rem;
    overflow: auto;

    > .table {
      flex: 1;
    }
  }
}
</style>

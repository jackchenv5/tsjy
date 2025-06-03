<script setup>
import { useResize } from '#/common/composables/resize'
import { dayjs, useLocale } from 'element-plus'
import { ref, watch } from 'vue'
import { getStatusRecordApi } from '../apis/facilityStatus'
import duration from 'dayjs/plugin/duration'

// region 组件
const { breakPoint } = useResize()
const { t } = useLocale()
const model = defineModel()
const props = defineProps(['facilityId', 'startTime', 'endTime'])
dayjs.extend(duration)
// endregion

// region 状态数据
const statusRecordData = ref({
  total: 0,
  items: [],
})
const loadingStatusRecord = ref(false)
const getStatusRecord = async () => {
  loadingStatusRecord.value = true
  try {
    const { data } = await getStatusRecordApi({
      facilityId: props.facilityId,
      startTime: props.startTime,
      endTime: props.endTime,
      pageIndex: pageIndex.value,
      pageSize: pageSize.value,
    })
    statusRecordData.value = data
  } catch (e) {
    console.error(e)
  } finally {
    loadingStatusRecord.value = false
  }
}
watch(model, async (value) => {
  if (value) {
    await getStatusRecord()
    model.value = false
  }
})
const getDurationString = (seconds) => {
  return dayjs.duration(seconds * 1000).format('HH:mm:ss')
}
const getStatusTagType = (status) => {
  switch (status) {
    case 'Invalid':
      return 'info'
    case 'Running':
      return 'success'
    case 'Standby':
      return 'primary'
    case 'Stopped':
      return 'warning'
    case 'Error':
      return 'danger'
  }
}
const getStatusString = (status) => {
  switch (status) {
    case 'Invalid':
      return t('el.facilityStatus.stoppedData.invalid')
    case 'Running':
      return t('el.facilityStatus.stoppedData.running')
    case 'Standby':
      return t('el.facilityStatus.stoppedData.standby')
    case 'Stopped':
      return t('el.facilityStatus.stoppedData.stopped')
    case 'Error':
      return t('el.facilityStatus.stoppedData.error')
  }
}
// endregion

// region 分页
const pageIndex = ref(1)
const pageSize = ref(20)
const onUpdateCurrentPage = async () => {
  await getStatusRecord()
}
const onUpdatePageSize = async () => {
  await getStatusRecord()
}
// endregion
</script>

<template>
  <div class="status-record-root" :class="breakPoint">
    <div class="header">
      {{ t('el.facilityStatus.stoppedData.statusRecord') }}
    </div>
    <ElTable
      :data="statusRecordData.items"
      class="table"
      v-loading="loadingStatusRecord"
      stripe
      show-overflow-tooltip
    >
      <ElTableColumn
        :label="t('el.facilityStatus.stoppedData.time')"
        min-width="180"
        prop="time"
      >
        <template #default="{ row }">
          {{ dayjs.unix(row['time']).format('YYYY-MM-DD HH:mm:ss') }}
        </template>
      </ElTableColumn>
      <ElTableColumn
        :label="t('el.facilityStatus.stoppedData.status')"
        width="100"
        prop="status"
      >
        <template #default="{ row }">
          <ElTag size="small" :type="getStatusTagType(row['status'])">
            {{ getStatusString(row['status']) }}
          </ElTag>
        </template>
      </ElTableColumn>
      <ElTableColumn
        :label="t('el.facilityStatus.stoppedData.duration')"
        width="120"
        prop="duration"
      >
        <template #default="{ row }">
          {{ getDurationString(row['duration']) }}
        </template>
      </ElTableColumn>
      <ElTableColumn
        :label="t('el.facilityStatus.stoppedData.shift')"
        width="120"
        prop="shift.name"
      >
      </ElTableColumn>
    </ElTable>
    <!--suppress JSValidateTypes -->
    <ElPagination
      layout="prev, pager, next, sizes, total"
      :total="statusRecordData.total"
      :default-page-size="20"
      :pager-count="5"
      v-model:current-page="pageIndex"
      @update:current-page="onUpdateCurrentPage"
      v-model:page-size="pageSize"
      :page-sizes="[20, 50, 100]"
      @update:page-size="onUpdatePageSize"
      :disabled="loadingStatusRecord"
    >
    </ElPagination>
  </div>
</template>

<style scoped lang="scss">
.status-record-root {
  flex: 1;
  display: flex;
  flex-flow: column;
  width: 0;
  min-height: 44rem;
  overflow: auto;
  border: 1px solid var(--el-border-color);
  border-radius: var(--el-border-radius-base);

  &.lg,
  &.md,
  &.sm,
  &.xs {
    width: auto;
  }

  > .header {
    background-color: var(--el-color-info-light-9);
    padding: 0.5rem 1rem;
    border-bottom: 1px solid var(--el-border-color);
    border-radius: var(--el-border-radius-base) var(--el-border-radius-base) 0 0;
  }

  > .table {
    flex: 1;
    margin-bottom: 1rem;
  }
}
</style>

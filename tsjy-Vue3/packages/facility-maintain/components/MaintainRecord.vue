<script setup>
import { useLocale } from 'element-plus'
import { onMounted, ref } from 'vue'
import { getMaintainHistoriesApi } from '../apis/maintainHistory'
import dayjs from 'dayjs'

// region 组件
const { t } = useLocale()
onMounted(async () => {
  await getMaintainRecords()
})
// endregion

// region 数据
const loadingMaintainRecords = ref(false)
const maintainRecordData = ref({
  total: 0,
  items: [],
})
const getMaintainRecords = async () => {
  loadingMaintainRecords.value = true
  try {
    const { data } = await getMaintainHistoriesApi({
      pageIndex: pageIndex.value,
      pageSize: pageSize.value,
    })
    maintainRecordData.value = data
  } catch (e) {
    console.error(e)
  } finally {
    loadingMaintainRecords.value = false
  }
}
const pageIndex = ref(1)
const pageSize = ref(20)
const onPageChange = async () => {
  await getMaintainRecords()
}
const onPageSizeChange = async () => {
  await getMaintainRecords()
}
// endregion
</script>

<template>
  <div class="maintain-record-root">
    <ElTable
      :data="maintainRecordData.items"
      class="table"
      stripe
      show-overflow-tooltip
    >
      <ElTableColumn
        :label="t('el.facilityMaintain.maintainHistory.partPosition')"
        prop="partPosition"
        min-width="240"
      >
      </ElTableColumn>
      <ElTableColumn
        :label="t('el.facilityMaintain.maintainHistory.partName')"
        prop="partName"
        width="200"
      >
      </ElTableColumn>
      <ElTableColumn
        :label="t('el.facilityMaintain.maintainHistory.maintainAt')"
        prop="maintainAt"
        width="180"
      >
        <template #default="{ row }">
          {{ dayjs.unix(row['maintainAt']).format('YYYY-MM-DD HH:mm:ss') }}
        </template>
      </ElTableColumn>
      <ElTableColumn
        :label="t('el.facilityMaintain.maintainHistory.username')"
        width="160"
        prop="username"
      >
      </ElTableColumn>
      <ElTableColumn
        :label="t('el.facilityMaintain.maintainHistory.fullName')"
        width="160"
        prop="fullName"
      >
      </ElTableColumn>
      <ElTableColumn
        :label="t('el.facilityMaintain.maintainHistory.maintainReason')"
        prop="maintainReason"
        min-width="240"
      >
      </ElTableColumn>
      <ElTableColumn
        :label="t('el.facilityMaintain.maintainHistory.maintainContent')"
        prop="maintainContent"
        min-width="240"
      >
      </ElTableColumn>
    </ElTable>
    <ElPagination
      layout="prev, pager, next, sizes, total"
      :total="maintainRecordData.total"
      :default-page-size="20"
      :pager-count="5"
      v-model:current-page="pageIndex"
      @update:current-page="onPageChange"
      v-model:page-size="pageSize"
      :page-sizes="[20, 50, 100]"
      @update:page-size="onPageSizeChange"
      :disabled="loadingMaintainRecords"
    >
    </ElPagination>
  </div>
</template>

<style scoped lang="scss">
.maintain-record-root {
  display: flex;
  flex-flow: column;
  grid-gap: 1rem;
  overflow: auto;

  .table {
    flex: 1;
    overflow: auto;
  }
}
</style>

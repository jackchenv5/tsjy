<script setup>
import { inject, nextTick, onMounted, ref, watch } from 'vue'
import { dayjs } from 'element-plus'
import { getProductionHistoryApi } from '@/apis/sawingProduction.js'

// region 组件
const startTime = inject('startTime')
const endTime = inject('endTime')
const currentFacility = inject('currentFacility')
const model = defineModel()
onMounted(async () => {
  await nextTick(async () => {
    await getHistoryData()
  })
})
watch(model, async (value) => {
  if (value) {
    await getHistoryData()
    model.value = false
  }
})
// endregion

// region 生产历史数据
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
    const dto = {
      facilityId: currentFacility.value.id,
      pageIndex: pageIndex.value,
      pageSize: pageSize.value,
      startTime: dayjs(startTime.value).unix(),
      endTime: dayjs(endTime.value).unix(),
    }
    const { data } = await getProductionHistoryApi(dto)
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

//生产记录完成时间排序
// 排序逻辑
const handleSortChange = ({ prop, order }) => {
  if (prop === 'completeTime') {
    historyData.value.items.sort((a, b) => {
      if (order === 'ascending') {
        return a.completeTime - b.completeTime; // 升序
      } else if (order === 'descending') {
        return b.completeTime - a.completeTime; // 降序
      }
      return 0; // 默认情况
    });
  }
};
// 默认按完成时间降序排序
historyData.value.items.sort((a, b) => b.completeTime - a.completeTime);
</script>

<template>
  <div class="container">
    <div class="header">生产记录</div>
    <div class="content">
      <ElTable
        class="table"
        stripe
        show-overflow-tooltip
        :data="historyData.items"
        @sort-change="handleSortChange"
        :default-sort="{ prop: 'completeTime', order: 'descending' }" 
      >
        <ElTableColumn label="客户代码" prop="customerCode" width="180">
          <template #default="{ row }">
            {{ row['customerCode'] || '-' }}
          </template>
        </ElTableColumn>
        <ElTableColumn
          label="材料规格"
          prop="materialSpecification"
          min-width="180"
        >
          <template #default="{ row }">
            {{ row['materialSpecification'] || '-' }}
          </template>
        </ElTableColumn>
        <ElTableColumn
          label="完成时间"
          prop="completeTime"
          width="180"
          sortable="custom"
        >
          <template #default="{ row }">
            <ElTag type="success" size="small">
              {{
                dayjs.unix(row['completeTime']).format('YYYY-MM-DD HH:mm:ss')
              }}
            </ElTag>
          </template>
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

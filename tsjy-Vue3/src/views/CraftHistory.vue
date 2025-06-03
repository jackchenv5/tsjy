<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import FacilityList from '#/facility/components/FacilityList.vue'
import { ref } from 'vue'
import { dayjs } from 'element-plus'
import { getCraftHistoryApi } from '@/apis/sawingCraft.js'

// region 当前设备
const currentFacility = ref()
const onFacilityChange = async (facility) => {
  currentFacility.value = facility
  await getCraftData()
}
// endregion

// region 时间
let startDate = dayjs()
  .subtract(6, 'day')
  .hour(0)
  .minute(0)
  .second(0)
  .millisecond(0)
const date = ref([
  new Date(startDate),
  new Date(new Date().setHours(23, 59, 59, 999)),
])
const defaultTime = [
  new Date(2000, 1, 1, 0, 0, 0),
  new Date(2000, 1, 1, 23, 59, 59),
]
const dateShortcuts = [
  {
    text: '最近 7 天',
    value: () => {
      const end = new Date()
      const start = new Date()
      start.setTime(start.getTime() - 6 * 24 * 60 * 60 * 1000)
      return [start, end]
    },
  },
  {
    text: '最近 14 天',
    value: () => {
      const end = new Date()
      const start = new Date()
      start.setTime(start.getTime() - 13 * 24 * 60 * 60 * 1000)
      return [start, end]
    },
  },
  {
    text: '最近 30 天',
    value: () => {
      const end = new Date()
      const start = new Date()
      start.setTime(start.getTime() - 29 * 24 * 60 * 60 * 1000)
      return [start, end]
    },
  },
]
const tempDate = ref([])
const handleCalendarChange = (val) => {
  tempDate.value = val
}
// endregion

// region 工艺数据
const loadingGetCraftData = ref(false)
const craftData = ref({
  total: 0,
  items: [],
})
const getCraftData = async () => {
  if (loadingGetCraftData.value) {
    return
  }
  loadingGetCraftData.value = true
  try {
    const dto = {
      pageIndex: pageIndex.value,
      pageSize: pageSize.value,
      facilityId: currentFacility.value['id'],
      startTime: dayjs(date.value[0]).unix(),
      endTime: dayjs(date.value[1]).unix(),
    }
    const { data } = await getCraftHistoryApi(dto)
    craftData.value = data
  } catch (e) {
    console.error(e)
  } finally {
    loadingGetCraftData.value = false
  }
}
// endregion

// region 分页
const pageIndex = ref(1)
const pageSize = ref(20)
const onUpdateCurrentPage = async () => {
  await getCraftData()
}
const onUpdatePageSize = async () => {
  await getCraftData()
}
// endregion

// region 筛选
const onFilterBtnClick = async () => {
  await getCraftData()
}
// endregion
</script>

<template>
  <ContentLayout>
    <template #header>工艺配置</template>
    <template #default>
      <FacilityList @change="onFacilityChange"></FacilityList>
      <div class="wrapper">
        <div class="header">
          <ElDatePicker
            style="width: 24rem"
            type="daterange"
            v-model="date"
            :default-time="defaultTime"
            :shortcuts="dateShortcuts"
            @calendar-change="handleCalendarChange"
            :clearable="false"
          >
          </ElDatePicker>
          <ElButton type="primary" plain @click="onFilterBtnClick">
            筛选
          </ElButton>
        </div>
        <ElTable
          stripe
          show-overflow-tooltip
          :data="craftData.items"
          row-key="id"
          class="table"
          default-expand-all
        >
          <ElTableColumn prop="time" label="时间" width="200">
            <template #default="{ row }">
              <ElTag type="info" size="small" v-if="row['id'] < 0">
                {{ dayjs.unix(row['time']).format('YYYY-MM-DD HH:mm:ss') }}
              </ElTag>
            </template>
          </ElTableColumn>
          <ElTableColumn prop="index" label="序号" width="80"></ElTableColumn>
          <ElTableColumn prop="sectionHeight" label="段高度" width="120">
            <template #default="{ row }">
              {{ row['sectionHeight']?.toFixed(3) }}
            </template>
          </ElTableColumn>
          <ElTableColumn prop="infeedVelocity" label="段进给速度" width="120">
            <template #default="{ row }">
              {{ row['infeedVelocity']?.toFixed(3) }}
            </template>
          </ElTableColumn>
          <ElTableColumn prop="newLineSpeed" label="新线进给量" width="120">
            <template #default="{ row }">
              {{ row['newLineSpeed']?.toFixed(3) }}
            </template>
          </ElTableColumn>
          <ElTableColumn prop="lineVelocity" label="段线速度" width="120">
            <template #default="{ row }">
              {{ row['lineVelocity']?.toFixed(3) }}
            </template>
          </ElTableColumn>
          <ElTableColumn prop="customerCode" label="客户代码" min-width="160">
            <template #default="{ row }">
              <ElTag type="primary" effect="plain" v-if="row['id'] < 0">
                {{ row['customerCode'] || '-' }}
              </ElTag>
            </template>
          </ElTableColumn>
          <ElTableColumn
            prop="materialSpecification"
            label="材料规格"
            min-width="160"
          >
            <template #default="{ row }">
              <ElTag type="warning" effect="plain" v-if="row['id'] < 0">
                {{ row['materialSpecification'] || '-' }}
              </ElTag>
            </template>
          </ElTableColumn>
        </ElTable>
        <ElPagination
          layout="prev, pager, next, sizes, total"
          :total="craftData.total"
          :default-page-size="20"
          :pager-count="5"
          v-model:current-page="pageIndex"
          @update:current-page="onUpdateCurrentPage"
          v-model:page-size="pageSize"
          :page-sizes="[20, 50, 100]"
          @update:page-size="onUpdatePageSize"
          :disabled="loadingGetCraftData"
        >
        </ElPagination>
      </div>
    </template>
  </ContentLayout>
</template>

<style scoped lang="scss">
.wrapper {
  flex: 1;
  display: flex;
  flex-flow: column;
  grid-gap: 1rem;
  overflow: auto;

  > .header {
    display: flex;
    flex-flow: row;
    grid-gap: 1rem;
    width: min-content;
  }

  .table {
    flex: 1;
  }
}
</style>

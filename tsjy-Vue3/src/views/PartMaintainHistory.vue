<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import FacilityList from '#/facility/components/FacilityList.vue'
import { ref } from 'vue'
import { dayjs } from 'element-plus'
import { getPartMaintainHistoryApi } from '@/apis/sawingPart.js'
import CreatePartMaintainHistory from '@/components/CreatePartMaintainHistory.vue'
import UpdatePartMaintainHistory from '@/components/UpdatePartMaintainHistory.vue'
import DeletePartMaintainHistory from '@/components/DeletePartMaintainHistory.vue'

// region 当前设备
const currentFacility = ref()
const onFacilityChange = async (facility) => {
  currentFacility.value = facility
  await getHistoryData()
}
// endregion

// region 时间
const date = ref([0, 0])
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

// region 筛选
const onFilterClick = async () => {
  await getHistoryData()
}
// endregion

// region 获取数据
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
  if (!date.value) {
    date.value = [0, 0]
  }
  try {
    const dto = {
      startTime: dayjs(date.value[0]).unix(),
      endTime: dayjs(date.value[1]).unix(),
      facilityId: currentFacility.value.id,
      pageIndex: pageIndex.value,
      pageSize: pageSize.value,
    }
    const { data } = await getPartMaintainHistoryApi(dto)
    historyData.value = data
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

// region 手动添加
const showCreatePartMaintainHistory = ref(false)
const onCreatePartMaintainHistoryClick = () => {
  showCreatePartMaintainHistory.value = true
}
const onCreatePartMaintainHistoryFinished = async () => {
  await getHistoryData()
}
// endregion

const currentPart = ref()
// region 手动更新
const showUpdatePartMaintainHistory = ref(false)
const onUpdatePartMaintainHistoryClick = (row) => {
  currentPart.value = JSON.parse(JSON.stringify(row))
  showUpdatePartMaintainHistory.value = true
}
const onUpdatePartMaintainHistoryFinished = async () => {
  await getHistoryData()
}
// endregion

// region 手动删除
const showDeletePartMaintainHistory = ref(false)
const onDeletePartMaintainHistoryClick = (row) => {
  currentPart.value = JSON.parse(JSON.stringify(row))
  showDeletePartMaintainHistory.value = true
}
const onDeletePartMaintainHistoryFinished = async () => {
  await getHistoryData()
}
// endregion
</script>

<template>
  <ContentLayout>
    <template #header>维护记录</template>
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
          >
          </ElDatePicker>
          <ElButton type="primary" plain @click="onFilterClick">
            筛选
          </ElButton>
          <ElDivider direction="vertical"></ElDivider>
          <ElButton type="primary" @click="onCreatePartMaintainHistoryClick">
            创建维护记录
          </ElButton>
        </div>
        <ElTable
          class="table"
          stripe
          show-overflow-tooltip
          :data="historyData.items"
        >
          <ElTableColumn label="零件名称" prop="partName" min-width="180">
          </ElTableColumn>
          <ElTableColumn label="备注" prop="remark" min-width="180">
          </ElTableColumn>
          <ElTableColumn label="维护原因" prop="reason" min-width="180">
          </ElTableColumn>
          <ElTableColumn label="处理方式" prop="processMethod" min-width="180">
          </ElTableColumn>
          <ElTableColumn prop="processTime" label="维护时间" width="180">
            <template #default="{ row }">
              <ElTag type="primary" size="small" effect="plain">
                {{ dayjs.unix(row['time']).format('YYYY-MM-DD HH:mm:ss') }}
              </ElTag>
            </template>
          </ElTableColumn>
          <ElTableColumn prop="time" label="记录时间" width="180">
            <template #default="{ row }">
              <ElTag type="info" size="small">
                {{ dayjs.unix(row['time']).format('YYYY-MM-DD HH:mm:ss') }}
              </ElTag>
            </template>
          </ElTableColumn>
          <ElTableColumn width="170">
            <template #default="{ row }">
              <ElButtonGroup size="small">
                <ElButton
                  type="primary"
                  plain
                  :disabled="row['reason'] === '计划维护'"
                  @click="onUpdatePartMaintainHistoryClick(row)"
                >
                  更新
                </ElButton>
                <ElButton
                  type="danger"
                  plain
                  @click="onDeletePartMaintainHistoryClick(row)"
                >
                  删除
                </ElButton>
              </ElButtonGroup>
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
        >
        </ElPagination>
      </div>
    </template>
  </ContentLayout>

  <CreatePartMaintainHistory
    v-model="showCreatePartMaintainHistory"
    @finished="onCreatePartMaintainHistoryFinished"
  >
  </CreatePartMaintainHistory>
  <UpdatePartMaintainHistory
    v-model="showUpdatePartMaintainHistory"
    :part="currentPart"
    @finished="onUpdatePartMaintainHistoryFinished"
  >
  </UpdatePartMaintainHistory>

  <DeletePartMaintainHistory
    v-model="showDeletePartMaintainHistory"
    :part="currentPart"
    @finished="onDeletePartMaintainHistoryFinished"
  >
  </DeletePartMaintainHistory>
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
    align-items: center;
  }

  .table {
    flex: 1;
  }
}
</style>

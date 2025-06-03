<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import { useLocale } from 'element-plus'
import FacilityList from '#/facility/components/FacilityList.vue'
import { ref } from 'vue'
import { getMaintainPlansApi } from '../apis/maintainPlan'
import dayjs from 'dayjs'
import MaintainContent from '../components/MaintainContent.vue'

// region 组件
const { t } = useLocale()
// endregion

// region 选择的设备变化
const currentFacility = ref()
const onFacilityChange = async (value) => {
  currentFacility.value = value
  if (currentFacility.value) {
    await getMaintainPlans()
  }
}
// endregion

// region 分页
const pageIndex = ref(1)
const pageSize = ref(20)
const onPageChange = async () => {
  await getMaintainPlans()
}
const onPageSizeChange = async () => {
  await getMaintainPlans()
}
// endregion

// region 筛选
const partNameFilter = ref()
const filter = async () => {
  await getMaintainPlans()
}
// endregion

// region 维护计划数据
const planData = ref({
  total: 0,
  items: [],
})
const loadingMaintainPlans = ref(false)
const getMaintainPlans = async () => {
  loadingMaintainPlans.value = true
  try {
    const { data } = await getMaintainPlansApi({
      facilityId: currentFacility.value['id'],
      pageIndex: pageIndex.value,
      pageSize: pageSize.value,
      partName: partNameFilter.value,
    })
    planData.value = data
  } catch (e) {
    console.error(e)
  } finally {
    loadingMaintainPlans.value = false
  }
}

/*
 * 维护计划状态
 * 0 - 计划中
 * 1 - 提醒
 * 2 - 到期
 * 3 - 超期
 */

const statusStringMap = (status) => {
  switch (status) {
    case 0:
      return t('el.facilityMaintain.maintainPlan.planing')
    case 1:
      return t('el.facilityMaintain.maintainPlan.remind')
    case 2:
      return t('el.facilityMaintain.maintainPlan.expired')
    case 3:
      return t('el.facilityMaintain.maintainPlan.overdue')
  }
}
const statusTagMap = {
  0: { type: 'success', effect: 'light' },
  1: { type: 'warning', effect: 'light' },
  2: { type: 'danger', effect: 'light' },
  3: { type: 'danger', effect: 'dark' },
}
// endregion

// region 填写维护内容
const showMaintainContent = ref(false)
const currentMaintainPlan = ref()
const onMaintain = (row) => {
  currentMaintainPlan.value = row
  showMaintainContent.value = true
}
const maintainFinished = async () => {
  await getMaintainPlans()
}
// endregion
</script>

<template>
  <ContentLayout>
    <template #header>
      {{ t('el.facilityMaintain.maintainPlans.maintainPlan') }}
    </template>
    <template #default>
      <FacilityList @change="onFacilityChange"></FacilityList>
      <div class="maintain-plans-container">
        <div class="filter-bar">
          <ElInput
            style="width: 20rem"
            :placeholder="t('el.facilityMaintain.maintainPlans.partName')"
            clearable
            v-model="partNameFilter"
          >
          </ElInput>
          <ElButton type="primary" plain @click="filter">
            {{ t('el.facilityMaintain.maintainPlans.filter') }}
          </ElButton>
        </div>
        <ElTable
          class="table"
          stripe
          :data="planData.items"
          show-overflow-tooltip
        >
          <ElTableColumn
            :label="t('el.facilityMaintain.maintainPlans.partName')"
            prop="facilityPart.name"
            min-width="160"
          >
          </ElTableColumn>
          <ElTableColumn
            :label="t('el.facilityMaintain.maintainPlans.partPosition')"
            prop="partPosition"
            min-width="240"
          >
          </ElTableColumn>
          <ElTableColumn
            :label="t('el.facilityMaintain.maintainPlans.createdAt')"
            prop="createdAt"
            width="150"
          >
            <template #default="{ row }">
              {{ dayjs.unix(row['createdAt']).format('YYYY-MM-DD') }}
            </template>
          </ElTableColumn>
          <ElTableColumn
            :label="t('el.facilityMaintain.maintainPlans.lastMaintainAt')"
            prop="lastMaintainAt"
            width="150"
          >
            <template #default="{ row }">
              {{ dayjs.unix(row['lastMaintainAt']).format('YYYY-MM-DD') }}
            </template>
          </ElTableColumn>
          <ElTableColumn
            :label="t('el.facilityMaintain.maintainPlans.nextMaintainAt')"
            prop="nextMaintainAt"
            width="150"
          >
            <template #default="{ row }">
              {{ dayjs.unix(row['nextMaintainAt']).format('YYYY-MM-DD') }}
            </template>
          </ElTableColumn>
          <ElTableColumn
            :label="
              t('el.facilityMaintain.maintainPlans.maintainCycleWithUnit')
            "
            prop="maintainCycle"
            width="180"
          >
          </ElTableColumn>

          <ElTableColumn
            :label="t('el.facilityMaintain.maintainPlans.status')"
            prop="status"
            min-width="90"
          >
            <template #default="{ row }">
              <ElTag
                :type="statusTagMap[row['status']].type"
                :effect="statusTagMap[row['status']].effect"
              >
                {{ statusStringMap(row['status']) }}
              </ElTag>
            </template>
          </ElTableColumn>
          <ElTableColumn width="100">
            <template #default="{ row }">
              <ElButton
                type="primary"
                plain
                size="small"
                @click="onMaintain(row)"
              >
                {{ t('el.facilityMaintain.maintainPlans.maintain') }}
              </ElButton>
            </template>
          </ElTableColumn>
        </ElTable>
        <ElPagination
          layout="prev, pager, next, sizes, total"
          :total="planData.total"
          :default-page-size="20"
          :pager-count="5"
          v-model:current-page="pageIndex"
          @update:current-page="onPageChange"
          v-model:page-size="pageSize"
          :page-sizes="[20, 50, 100]"
          @update:page-size="onPageSizeChange"
          :disabled="loadingMaintainPlans"
        >
        </ElPagination>
      </div>
    </template>
  </ContentLayout>

  <MaintainContent
    v-model="showMaintainContent"
    :plan-id="currentMaintainPlan?.id"
    @finished="maintainFinished"
  >
  </MaintainContent>
</template>

<style scoped lang="scss">
.maintain-plans-container {
  display: flex;
  flex-flow: column;
  grid-gap: 1rem;
  height: 100%;
  overflow: auto;

  > .filter-bar {
    display: flex;
    flex-flow: row wrap;
    grid-gap: 1rem;
  }

  > .table {
    flex: 1;
    margin-bottom: 1rem;
  }
}
</style>

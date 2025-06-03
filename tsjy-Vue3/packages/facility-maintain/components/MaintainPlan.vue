<script setup>
import { ref, watch } from 'vue'
import { deleteMaintainPlanApi, getMaintainPlanApi } from '../apis/maintainPlan'
import { ElMessage, useLocale } from 'element-plus'
import AddMaintainPlan from './AddMaintainPlan.vue'
import dayjs from 'dayjs'

// region 组件
const props = defineProps({
  part: {
    type: Object,
    require: true,
  },
})
const { t } = useLocale()
// endregion

// region 维护计划数据
const maintainPlan = ref()
const loadingMaintainPlan = ref(false)
const getMaintainPlan = async () => {
  loadingMaintainPlan.value = true
  try {
    const { data } = await getMaintainPlanApi(props.part['id'])
    maintainPlan.value = data
  } catch (e) {
    console.error(e)
  } finally {
    loadingMaintainPlan.value = false
  }
}
watch(
  () => props.part,
  async () => {
    await getMaintainPlan()
  },
  { immediate: true },
)

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

// region 添加维护计划
const showAddMaintainPlan = ref(false)
const onAddMaintainPlan = () => {
  showAddMaintainPlan.value = true
}
const onAddMaintainPlanFinished = async () => {
  await getMaintainPlan()
}
// endregion

// region 删除维护计划
const loadingDeleteMaintainPlan = ref(false)
const deleteMaintainPlan = async () => {
  loadingDeleteMaintainPlan.value = true
  try {
    await deleteMaintainPlanApi(maintainPlan.value['id'])
    await getMaintainPlan()
    ElMessage({
      message: t('el.facilityMaintain.maintainPlan.deleteSuccess'),
      type: 'success',
    })
  } catch (e) {
    ElMessage({
      message: t('el.facilityMaintain.maintainPlan.deleteFailed'),
      type: 'error',
    })
    console.error(e)
  } finally {
    loadingDeleteMaintainPlan.value = false
  }
}
// endregion
</script>

<template>
  <div class="maintain-plan-root">
    <div class="plan" v-if="maintainPlan">
      <ElDescriptions
        border
        size="small"
        :column="4"
        direction="vertical"
        :title="t('el.facilityMaintain.maintainPlan.maintainPlan')"
      >
        <template #extra>
          <ElPopconfirm
            :title="t('el.facilityMaintain.maintainPlan.deleteConfirm')"
            @confirm="deleteMaintainPlan"
            style="margin-top: 1rem"
          >
            <template #reference>
              <ElButton type="danger" size="small" plain>
                {{ t('el.facilityMaintain.maintainPlan.delete') }}
              </ElButton>
            </template>
          </ElPopconfirm>
        </template>
        <ElDescriptionsItem
          :label="t('el.facilityMaintain.maintainPlan.createdAt')"
        >
          {{ dayjs.unix(maintainPlan['createdAt']).format('YYYY-MM-DD') }}
        </ElDescriptionsItem>
        <ElDescriptionsItem
          :label="t('el.facilityMaintain.maintainPlan.lastMaintainAt')"
        >
          {{ dayjs.unix(maintainPlan['lastMaintainAt']).format('YYYY-MM-DD') }}
        </ElDescriptionsItem>
        <ElDescriptionsItem
          :label="t('el.facilityMaintain.maintainPlan.nextMaintainAt')"
        >
          {{ dayjs.unix(maintainPlan['nextMaintainAt']).format('YYYY-MM-DD') }}
        </ElDescriptionsItem>
        <ElDescriptionsItem
          :label="t('el.facilityMaintain.maintainPlan.status')"
        >
          <ElTag
            :type="statusTagMap[maintainPlan['status']].type"
            :effect="statusTagMap[maintainPlan['status']].effect"
          >
            {{ statusStringMap(maintainPlan['status']) }}
          </ElTag>
        </ElDescriptionsItem>
      </ElDescriptions>
    </div>
    <div class="create-plan" v-else>
      <ElButton type="primary" plain @click="onAddMaintainPlan">
        {{ t('el.facilityMaintain.maintainPlan.addMaintainPlan') }}
      </ElButton>
    </div>
  </div>

  <AddMaintainPlan
    v-model="showAddMaintainPlan"
    :part-id="part['id']"
    :facility-id="part['facilityId']"
    :maintain-cycle="Number(part['maintainCycle'])"
    @finished="onAddMaintainPlanFinished"
  >
  </AddMaintainPlan>
</template>

<style scoped lang="scss"></style>

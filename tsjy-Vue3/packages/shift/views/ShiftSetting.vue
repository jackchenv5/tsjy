<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import { onMounted, ref } from 'vue'
import { getShiftApi, updateShiftApi } from '../apis/shift'
import { dayjs, ElMessage, useLocale } from 'element-plus'
import { useShiftStore } from '../stores/shiftStore'

// region 组件
const { t } = useLocale()
onMounted(async () => {
  await getShift()
})
// endregion

// region 获取班次数据
const shifts = ref([])
const loadingGetShift = ref(false)
const getShift = async () => {
  loadingGetShift.value = true
  try {
    const { data } = await getShiftApi()
    shifts.value = []
    for (let shift of data) {
      shift.startTime = dayjs.unix(shift.startTime).toDate()
      shift.endTime = dayjs.unix(shift.endTime).toDate()
      shifts.value.push(shift)
    }
  } catch (error) {
    console.error(error)
  } finally {
    loadingGetShift.value = false
  }
}
// endregion

// region 更新班次数据
const loadingUpdateShift = ref(false)
const shiftStore = useShiftStore()
const updateShift = async () => {
  let newShifts = []
  for (const shift of shifts.value) {
    let newShift = {
      id: shift.id,
      number: shift['number'],
      isEnabled: shift['isEnabled'],
      name: shift.name,
      spanTheDay: shift['spanTheDay'],
      startTime: dayjs(shift['startTime']).unix(),
      endTime: dayjs(shift['endTime']).unix(),
    }
    newShifts.push(newShift)
  }

  loadingUpdateShift.value = true
  try {
    await updateShiftApi(newShifts)
    ElMessage({
      message: t('el.shift.setting.updateSuccessful'),
      type: 'success',
    })
    await shiftStore.getCurrentShift()
  } catch (e) {
    ElMessage({
      message: t('el.shift.updateFailed'),
      type: 'error',
    })
    console.error(e)
  } finally {
    loadingUpdateShift.value = false
  }
}
// endregion
</script>

<template>
  <ContentLayout>
    <template #header>
      {{ t('el.shift.setting.shiftSetting') }}
    </template>
    <template #default>
      <div class="card-container">
        <div class="card" v-for="(shift, index) of shifts" :key="index">
          <div class="header">
            {{ t('el.shift.setting.title') + shift.number }}
          </div>
          <div class="content">
            <ElForm label-position="top">
              <ElFormItem :label="t('el.shift.setting.isEnabled')">
                <ElSwitch v-model="shift.isEnabled"></ElSwitch>
              </ElFormItem>
              <ElFormItem :label="t('el.shift.setting.shiftName')">
                <ElInput v-model="shift.name"></ElInput>
              </ElFormItem>
              <ElFormItem :label="t('el.shift.setting.startTime')">
                <ElTimePicker
                  format="HH:mm"
                  :editable="false"
                  v-model="shift.startTime"
                >
                </ElTimePicker>
              </ElFormItem>
              <ElFormItem :label="t('el.shift.setting.spanTheDay')">
                <ElSwitch v-model="shift['spanTheDay']"></ElSwitch>
              </ElFormItem>
              <ElFormItem :label="t('el.shift.setting.endTime')">
                <ElTimePicker
                  format="HH:mm"
                  :editable="false"
                  v-model="shift.endTime"
                >
                </ElTimePicker>
              </ElFormItem>
            </ElForm>
          </div>
        </div>
      </div>
    </template>
    <template #footer>
      <ElButton
        type="primary"
        :loading="loadingUpdateShift"
        @click="updateShift"
      >
        {{ t('el.shift.setting.update') }}
      </ElButton>
    </template>
  </ContentLayout>
</template>

<style scoped lang="scss">
.card-container {
  flex: 1;
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(32rem, 1fr));
  grid-gap: 1rem;
  overflow: auto;

  > .card {
    border: 1px solid var(--el-border-color);
    border-radius: var(--el-border-radius-base);

    > .header {
      background-color: var(--el-color-info-light-9);
      display: flex;
      align-items: center;
      padding: 0.5rem 1rem;
      border-top-left-radius: var(--el-border-radius-base);
      border-top-right-radius: var(--el-border-radius-base);
      border-bottom: 1px solid var(--el-border-color);
    }

    > .content {
      padding: 1rem;
      border-radius: var(--el-border-radius-base);
    }
  }
}
</style>

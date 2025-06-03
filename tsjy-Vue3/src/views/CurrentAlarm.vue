<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import { onBeforeUnmount, onMounted, ref } from 'vue'
import { getCurrentAlarmsApi } from '@/apis/sawingMachineAlarm.js'
import dayjs from 'dayjs'
import { Refresh, Loading } from '@element-plus/icons-vue'

// region 组件
onMounted(async () => {
  await getCurrentAlarms()
  startInterval()
})
onBeforeUnmount(() => {
  stopInterval()
})
// endregion

// region 获取报警数据
const loadingGetCurrentAlarms = ref(false)
const currentAlarms = ref()
const getCurrentAlarms = async () => {
  if (loadingGetCurrentAlarms.value) {
    return
  }
  loadingGetCurrentAlarms.value = true
  try {
    const { data } = await getCurrentAlarmsApi()
    currentAlarms.value = data
  } catch (e) {
    console.error(e)
  } finally {
    loadingGetCurrentAlarms.value = false
  }
}
// endregion

// region 自动刷新
const autoFresh = ref(true)
const handleAutoRefreshChange = async () => {
  if (autoFresh.value) {
    await getCurrentAlarms()
    startInterval()
  } else {
    stopInterval()
  }
}
let intervalId
const startInterval = () => {
  stopInterval()
  intervalId = setInterval(async () => {
    if (autoFresh.value) {
      await getCurrentAlarms()
    }
  }, 2000)
}
const stopInterval = () => {
  if (intervalId) {
    clearInterval(intervalId)
  }
}
// endregion
</script>

<template>
  <ContentLayout>
    <template #header> 当前报警</template>
    <template #default>
      <div class="content">
        <div class="header">
          <ElButton
            type="primary"
            plain
            :icon="Refresh"
            :disabled="autoFresh || loadingGetCurrentAlarms"
            @click="getCurrentAlarms"
          >
            刷新
          </ElButton>
          <ElCheckbox
            label="自动刷新"
            v-model="autoFresh"
            @change="handleAutoRefreshChange"
          >
          </ElCheckbox>
          <div class="loading-icon" v-show="loadingGetCurrentAlarms">
            <ElIcon class="is-loading">
              <Loading></Loading>
            </ElIcon>
          </div>
        </div>
        <ElTable
          class="table"
          stripe
          show-overflow-tooltip
          :data="currentAlarms"
        >
          <ElTableColumn label="设备名称" prop="facility.name" width="180">
          </ElTableColumn>
          <ElTableColumn label="开始时间" prop="startTime" width="180">
            <template #default="{ row }">
              <ElTag type="info" size="small">
                {{ dayjs.unix(row['startTime']).format('YYYY-MM-DD HH:mm:ss') }}
              </ElTag>
            </template>
          </ElTableColumn>
          <ElTableColumn label="报警消息" prop="message" min-width="240">
          </ElTableColumn>
        </ElTable>
      </div>
    </template>
  </ContentLayout>
</template>

<style scoped lang="scss">
.content {
  flex: 1;
  display: flex;
  flex-flow: column;
  grid-gap: 1rem;

  > .header {
    margin-bottom: 1rem;
    display: flex;
    flex-flow: row wrap;
    grid-gap: 1rem;

    > .loading-icon {
      display: flex;
      align-items: center;
      color: var(--el-color-primary);
      font-size: 2rem;
    }
  }

  > .table {
    flex: 1;
  }
}
</style>

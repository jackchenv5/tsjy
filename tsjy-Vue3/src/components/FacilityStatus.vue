<script setup>
import { onBeforeUnmount, onMounted, ref, toValue, watch } from 'vue'
import { useLocale } from 'element-plus'
import { useResize } from '#/common/composables/resize.js'
import { getCurrentShiftStatusApi } from '#/facility-status/apis/facilityStatus.js'
import { getSawingMachineStatusApi } from '@/apis/sawingMachineStatus.js'
import DataItem from '@/components/DataItem.vue'
import { useCssVar, useDark } from '@vueuse/core'

// region 组件
const { t } = useLocale()
const { breakPoint } = useResize()
const props = defineProps({
  facility: {
    type: Object,
    require: true,
  },
})
onMounted(async () => {
  try {
    await getCurrentShiftStatus()
    await getSawingMachineStatus()
    startInterval();
  } catch (error) {
    console.error('初始化失败',error);
  }
});
onBeforeUnmount(() => {
  stopInterval()
})
// endregion

// region 数据
/*
    Invalid = 0,
    Running = 1,
    Standby = 2,
    Stopped = 3,
    Error = 4
*/

const currentStatus = ref(0)
const loading = ref(false)
const getCurrentShiftStatus = async () => {
  //对设备的唯一表示ID进行判断，非控才能进行下一步
  if (!props.facility['id']) {
    return
  }
  loading.value = true
  try {
    const { data } = await getCurrentShiftStatusApi(props.facility['id'])
    currentStatus.value = data['currentStatus']
  } catch (e) {
    console.error(e)
  } finally {
    loading.value = false
  }
}
let interval
const startInterval = () => {
  stopInterval()
  interval = setInterval(async () => {
    await getCurrentShiftStatus()
    await getSawingMachineStatus()
  }, 2000)
}
const stopInterval = () => {
  if (interval) {
    clearInterval(interval)
  }
}
const getStatusString = () => {
  switch (currentStatus.value) {
    case 0:
      return t('el.facilityStatus.stoppedData.invalid')
    case 1:
      return t('el.facilityStatus.stoppedData.running')
    case 2:
      return t('el.facilityStatus.stoppedData.standby')
    case 3:
      return t('el.facilityStatus.stoppedData.stopped')
    case 4:
      return t('el.facilityStatus.stoppedData.error')
  }
}
// endregion

// region 图片
const imgUrl = ref(new URL('@/assets/JYKJ.png', import.meta.url).href)
const getImageUrl = (facilityId) => {
  // 从 localStorage 获取设备的图片路径
  const customImgUrl = localStorage.getItem(`facility-${facilityId}`);
  return customImgUrl || imgUrl.value;
};
// endregion

// region 切割机特定状态
const sawingMachineStatus = ref({
  autoStepNum: 0,
  lineVelocity: 0,
  newLineLength: 0,
  currentFeedSpeed: 0,
  totalCutHeight: 0,
  cutHeight: 0,
  tensionValueTop: 0,
  tensionValueBottom: 0,
  mainWheelDiameter: 0,
  customerCode: '-',
  materialSpecification: '-',
  cutPercentage: 0,
  totalCutTime: '00:00:00',
  remainingTime: '00:00:00',
})
const loadingSawingMachineStatus = ref(false)
const getSawingMachineStatus = async () => {
  if (!props.facility['id']) {
    return
  }
  loadingSawingMachineStatus.value = true
  try {
    const { data } = await getSawingMachineStatusApi(props.facility['id'])
    sawingMachineStatus.value = data
  } catch (e) {
    console.error(e)
  } finally {
    loadingSawingMachineStatus.value = false
  }
}
// endregion

// region 数据标签样式

const statusColor = ref()
const statusBgColor = ref()
const statusMarkColor = ref()

watch(currentStatus, () => {
  updateStatusColors()
})

const updateStatusColors = () => {
  switch (currentStatus.value) {
    case 1:
      statusColor.value = toValue(useCssVar('--green-7'))
      statusBgColor.value = toValue(useCssVar('--green-2'))
      statusMarkColor.value = toValue(useCssVar('--green-6'))
      break
    case 2:
      statusColor.value = toValue(useCssVar('--blue-7'))
      statusBgColor.value = toValue(useCssVar('--blue-2'))
      statusMarkColor.value = toValue(useCssVar('--blue-6'))
      break
    case 3:
      statusColor.value = toValue(useCssVar('--gold-7'))
      statusBgColor.value = toValue(useCssVar('--gold-2'))
      statusMarkColor.value = toValue(useCssVar('--gold-6'))
      break
    case 4:
      statusColor.value = toValue(useCssVar('--red-7'))
      statusBgColor.value = toValue(useCssVar('--red-2'))
      statusMarkColor.value = toValue(useCssVar('--red-6'))
      break
    default:
      statusColor.value = toValue(useCssVar('--gray-8'))
      statusBgColor.value = toValue(useCssVar('--gray-2'))
      statusMarkColor.value = toValue(useCssVar('--gray-8'))
      break
  }
}

useDark({
  onChanged: () => {
    updateStatusColors()
  },
})
// endregion

// region 设备详情
const showDetail = ref(false)
const showDetailClick = () => {
  showDetail.value = true
}
// endregion
</script>

<template>
  <div class="facility-status">
    <div class="header">
      <span>{{ facility['name'] }}</span>
      <span class="status" :class="`status-${currentStatus}`">{{ getStatusString() }}</span>
      <ElButton type="primary" link size="small" @click="showDetailClick">
        详细信息
      </ElButton>
    </div>
    <div class="content" :class="breakPoint">
      <div class="data">
        <DataItem label="设备编号" :content="sawingMachineStatus.autoStepNum">
        </DataItem>
        <DataItem
          label="速度"
          :content="sawingMachineStatus.lineVelocity.toFixed(3)"
        >
        </DataItem>
        <DataItem
          label="振动"
          :content="sawingMachineStatus.cutPercentage.toFixed(3)"
        >
        </DataItem>

        <DataItem
          label="张力"
          :content="sawingMachineStatus.totalCutTime"
        >
        </DataItem>
        <DataItem label="跟随误差" :content="sawingMachineStatus.remainingTime">
        </DataItem>
      </div>
      <div class="img">
        <img :src="getImageUrl(props.facility.id)" alt="设备图片" />
      </div>
    </div>

    <ElDialog
      v-model="showDetail"
      :title="facility['name'] + ' 详情'"
      class="dialog"
      :class="breakPoint"
    >
      <div class="detail">
        <div class="img">
          <img :src="getImageUrl(props.facility.id)" alt="弹窗中设备图片" />
        </div>
        <div class="data">
          <DataItem label="设备编号" :content="sawingMachineStatus.autoStepNum">
          </DataItem>
          <DataItem
            label="切线速度"
            :content="sawingMachineStatus.lineVelocity.toFixed(3)"
          >
          </DataItem>
          <DataItem
            label="切割百分比"
            :content="sawingMachineStatus.cutPercentage"
          >
          </DataItem>
          <DataItem
            label="切割总时间"
            :content="sawingMachineStatus.totalCutTime"
          >
          </DataItem>
          <DataItem
            label="剩余时间"
            :content="sawingMachineStatus.remainingTime"
          >
          </DataItem>
          <DataItem
            label="新线进给量"
            :content="sawingMachineStatus.newLineLength.toFixed(3)"
          >
          </DataItem>
          <DataItem
            label="进给速度"
            :content="sawingMachineStatus.currentFeedSpeed.toFixed(3)"
          >
          </DataItem>
          <DataItem
            label="切割总高度"
            :content="sawingMachineStatus.totalCutHeight.toFixed(3)"
          >
          </DataItem>
          <DataItem
            label="已切割高度"
            :content="sawingMachineStatus.cutHeight.toFixed(3)"
          >
          </DataItem>
          <DataItem
            label="上摆杆张力设定值"
            :content="sawingMachineStatus.tensionValueTop.toFixed(3)"
          >
          </DataItem>
          <DataItem
            label="下摆杆张力设定值"
            :content="sawingMachineStatus.tensionValueBottom.toFixed(3)"
          >
          </DataItem>
          <DataItem
            label="槽轮直径"
            :content="sawingMachineStatus.mainWheelDiameter.toFixed(3)"
          >
          </DataItem>
          <DataItem
            label="客户代码"
            :content="sawingMachineStatus.customerCode"
          >
          </DataItem>
          <DataItem
            label="切割材料规格"
            :content="sawingMachineStatus.materialSpecification"
          >
          </DataItem>
        </div>
      </div>
    </ElDialog>
  </div>
</template>

<style scoped lang="scss">
.facility-status {
  border: 1px solid var(--el-border-color);
  border-radius: var(--el-border-radius-base);

  > .header {
    display: flex;
    flex-flow: row;
    grid-gap: 1rem;
    align-items: center;
    background-color: var(--el-color-info-light-9);
    padding: 0.5rem 1rem;
    border-radius: var(--el-border-radius-base) var(--el-border-radius-base) 0 0;
    border-bottom: 1px solid var(--el-border-color);

    > .status {
      padding: 0 1rem;
      border-radius: 0.5rem;
      color: white;
    }

    > .status-0 {
      background-color: var(--gray-6);
    }

    > .status-1 {
      background-color: var(--green-6);
    }

    > .status-2 {
      background-color: var(--blue-6);
    }

    > .status-3 {
      background-color: var(--gold-6);
    }

    > .status-4 {
      background-color: var(--red-6);
    }
  }

  > .content {
    display: flex;
    flex-flow: row;
    grid-gap: 1rem;
    padding: 1rem;

    > .data {
      flex: 1;
      width: 12rem;
      display: flex;
      flex-flow: column;
      grid-gap: 1rem;
    }

    > .img {
      flex: 4;
      object-fit: contain;

      > img {
        width: 100%;
        height: 100%;
        object-fit: cover;
      }
    }
  }
}
</style>

<style lang="scss">
.facility-status {
  .dialog {
    width: 80rem;

    .detail {
      display: flex;
      flex-flow: row;
      grid-gap: 1rem;

      > .data {
        flex: 1;
        display: grid;
        grid-template-columns: repeat(2, minmax(12rem, 1fr));
        grid-gap: 1rem;
      }

      > .img {
        flex: 4;
        object-fit: contain;

        > img {
          width: 100%;
          height: 100%;
          object-fit: cover;
        }
      }
    }

    &.xs,
    &.sm {
      width: 32rem;

      .detail {
        > .img {
          display: none;
        }
      }
    }

    &.md {
      width: 72rem;
    }
  }
}
</style>

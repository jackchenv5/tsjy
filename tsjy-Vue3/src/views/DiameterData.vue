<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import FacilityList from '#/facility/components/FacilityList.vue'
import { ref } from 'vue'
import { getMotorsApi } from '@/apis/sawingMotor.js'
import MotorItem from '@/components/MotorItem.vue'
import { dayjs } from 'element-plus'
import { Loading } from '@element-plus/icons-vue'
import RollDiameterRealtimeData from '@/components/RollDiameterRealtimeData.vue'
import RollDiameterHistoryData from '@/components/RollDiameterHistoryData.vue'


// region 选中设备
const currentFacility = ref()
const facilityChange = async (facility) => {
  currentFacility.value = facility
  await getMotors()
  // console.log(currentFacility.value.id)
}
// endregion

// region 获取电机
const loadingGetMotors = ref(false)
const motors = ref()
// const getMotors = async () => {
//   if (loadingGetMotors.value) {
//     return
//   }
//   loadingGetMotors.value = true
//   try {
//     const { data } = await getMotorsApi(currentFacility.value['id'])
//     motors.value = data
//     if (motors.value.length > 0) {
//       currentMotor.value = motors.value[0]
//     }
//   } catch (e) {
//     console.error(e)
//   } finally {
//     loadingGetMotors.value = false
//   }
// }
const getMotors = async () => {
  if (loadingGetMotors.value) {
    return
  }
  loadingGetMotors.value = true
  try {
    const { data } = await getMotorsApi(currentFacility.value['id'])
    // 过滤出名字包含“收卷”或“放卷”的电机
    motors.value = data.filter(motor => 
      motor.name && (motor.name.includes('收卷') || motor.name.includes('放卷'))
    )
    if (motors.value.length > 0) {
      currentMotor.value = motors.value[0]
    }
  } catch (e) {
    console.error(e)
  } finally {
    loadingGetMotors.value = false
  }
}
// endregion

// region 处理当前选中的电机
const currentMotor = ref()
const motorClick = (motor) => {
  currentMotor.value = motor
  // console.log(currentMotor,currentMotor.value.id)
  refreshComponent();
}
// endregion

// region 电机数据类型
const motorDataTypeOptions = [
  { label: '实时数据', value: 'realtime' },
  { label: '历史数据', value: 'history' },
]
const currentMotorDataType = ref('realtime')
// endregion

// region 日期
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
// endregion

// region 查询历史数据
const loadingHistoryData = ref(false)
const onQueryButtonClick = () => {
  loadingHistoryData.value = true
}
// endregion

//获取子组件传递回来的历史数据，以此来处理历史数据导出
const historyData4Echarts = ref(null)
const handleDataLoaded = (data) =>{
  historyData4Echarts.value = data;  //保存从子组件中获取到的数据
}

//强制刷新实时数据保证阈值能够同步修改
const componentKey = ref(0);
const refreshComponent = () => {
  componentKey.value += 1; // 改变 key 的值，触发重新渲染
};

//历史数据导出测试数据
// const motorHistoryData = ref({
//   vibrationData: [{
//     time: '2025-3-13 17:04',
//     value: 4,
//   }],
//   tensionData: [{
//     time: '2025-3-13 17:04',
//     value: 5,
//   }],
//   followErrorData: [{
//     time: '2025-3-13 17:04',
//     value: 6,
//   }],
//   temperatureErrorData: [{
//     time: '2025-3-13 17:04',
//     value: 7,
//   }],
//   currentErrorData: [{
//     time: '2025-3-13 17:04',
//     value: 8,
//   }],
// })

//将数据转换为CSV格式的字符串
function convertToCSV(data) {
  const headers = ['Time', 'Vibration', 'Tension', 'Follow Error', 'Temperature', 'Current'];
  let csvContent = headers.join(',') + '\n';

  // 假设所有数据的时间戳相同，以 vibrationData 的时间戳为准
  data.vibrationData.forEach((item, index) => {
    const row = [
      dayjs.unix(item.time).format('YYYY-MM-DD HH:mm:ss'),
      item.value,
      data.tensionData[index]?.value,
      data.followErrorData[index]?.value,
      data.temperatureErrorData[index]?.value,
      data.currentErrorData[index]?.value
    ];
    csvContent += row.join(',') + '\n';
  });
  return csvContent;
}

//创建Blob对象并触发下载
function exportHistoryData() {
  const csvContent = convertToCSV(historyData4Echarts.value);
  const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
  const url = URL.createObjectURL(blob);

  const link = document.createElement('a');
  link.href = url;
  link.download = dayjs().format('YYYY-MM-DD HH:mm') + '电机历史数据.csv'; // 设置下载文件名
  link.style.visibility = 'hidden';
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
}
</script>

<template>
  <ContentLayout>
    <template #header>印刷卷径检测</template>
    <template #default>
      <!-- 触发设备栏设备切换事件，刷新当前选中设备信息 -->
      <FacilityList @change="facilityChange"></FacilityList>
      <!-- 电机/轴列表，当电机/轴列表中不为空时，将其内容导入，自带滚动条空间，可自适应 -->
      <div class="container" v-if="motors?.length > 0">
        <ElScrollbar class="motors-container">
          <div class="motors-container">
            <!-- 调用MotorItem Components中的组件，对显示内容进行格式渲染、点击切换显示内容，电机配置信息同步等等 -->
            <MotorItem
              class="motor"
              v-for="(motor, index) of motors"
              :key="index"
              :motor="motor"
              :active="motor === currentMotor"
              @click="motorClick(motor)"
            >
            </MotorItem>
          </div>
        </ElScrollbar>
        <div class="data-container">
          <div class="header">
            <ElSegmented
              :options="motorDataTypeOptions"
              v-model="currentMotorDataType"
            >
            </ElSegmented>

            <div
              class="query-container"
              v-if="currentMotorDataType === 'history'"
            >
              <!-- 日期选择框 -->
              <ElDatePicker
                style="width: 24rem"
                type="daterange"
                v-model="date"
                :default-time="defaultTime"
                :shortcuts="dateShortcuts"
              >
              </ElDatePicker>
              <!-- 查询按钮 -->
              <ElButton
                type="primary"
                plain
                @click="onQueryButtonClick"
                :disabled="loadingHistoryData"
              >
                查询
              </ElButton>
              <!-- 导出历史数据按钮 -->
              <ElButton
                type="primary"
                plain
                @click="exportHistoryData"
                :disabled="!historyData4Echarts"
              >
                导出历史数据
              </ElButton>
              <!-- 等待数据时显示内容 -->
              <ElIcon class="is-loading" v-show="loadingHistoryData" :size="24">
                <Loading></Loading>
              </ElIcon>
            </div>
          </div>
          <!-- 实时数据页面调用 -->
          <RollDiameterRealtimeData
            v-if="currentMotorDataType === 'realtime'"
            :key="componentKey"
            :motor-id="currentMotor.id"
            :motor="currentMotor"
          >
          </RollDiameterRealtimeData>
          <!-- 历史数据页面调用 -->
          <RollDiameterHistoryData
            v-else
            :motor-id="currentMotor.id"
            :start-time="date[0]"
            :end-time="date[1]"
            v-model="loadingHistoryData"
            @data-loaded="handleDataLoaded"
          >
          </RollDiameterHistoryData>
        </div>
      </div>
      <ElEmpty v-else>
        <template #description>
          <ElText>请先</ElText>
          <RouterLink to="/tsjy-motor/setting">添加电机</RouterLink>
        </template>
      </ElEmpty>
    </template>
  </ContentLayout>
</template>

<style scoped lang="scss">
.container {
  flex: 1;
  display: flex;
  flex-flow: row;
  overflow: auto;

  .motors-container {
    display: flex;
    flex-flow: column;
    grid-gap: 1rem;
    flex-shrink: 0;

    > .motor {
      margin-right: 1rem;
    }
  }

  .data-container {
    flex: 1;
    display: flex;
    flex-flow: column;
    grid-gap: 1rem;

    .header {
      display: flex;
      flex-flow: row;
      grid-gap: 1rem;
      width: min-content;

      > .query-container {
        display: flex;
        flex-flow: row;
        grid-gap: 1rem;
        align-items: center;

        > .is-loading {
          color: var(--el-color-primary);
        }
      }
    }
  }
}
</style>

<script setup>
import ContentLayout from '#/common/layouts/ContentLayout.vue'
import FacilityList from '#/facility/components/FacilityList.vue'
import { ref } from 'vue'
import { getMotorsApi } from '@/apis/sawingMotor.js'
import AddMotor from '@/components/AddMotor.vue'
import MotorItem from '@/components/MotorItem.vue'
import MotorProperty from '@/components/MotorProperty.vue'
import UpdateMotor from '@/components/UpdateMotor.vue'
import DeleteMotor from '@/components/DeleteMotor.vue'

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
const getMotors = async () => {
  if (loadingGetMotors.value) {
    return
  }
  loadingGetMotors.value = true
  try {
    const { data } = await getMotorsApi(currentFacility.value['id'])
    motors.value = data
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
  // console.log(currentMotor.value.id)
}
// endregion

// region 添加电机对话框
const showAddMotor = ref(false)
const onAddMotorClick = () => {
  showAddMotor.value = true
}
const addMotorFinished = async () => {
  await getMotors()
}
// endregion

// region 更新电机对话框
const showUpdateMotor = ref(false)
const onUpdateMotorClick = () => {
  showUpdateMotor.value = true
}
const updateMotorFinished = async () => {
  await getMotors()
}
// endregion

// region 删除电机对话框
const showDeleteMotor = ref(false)
const onDeleteMotorClick = () => {
  showDeleteMotor.value = true
}
const deleteMotorFinished = async () => {
  await getMotors()
}
// endregion
</script>

<template>
  <ContentLayout>
    <template #header>电机设置</template>
    <template #default>
      <FacilityList @change="facilityChange"></FacilityList>
      <div class="container" v-loading="!currentFacility">
        <div class="header">
          <ElButton type="primary" plain @click="onAddMotorClick">
            添加电机
          </ElButton>
          <ElDivider direction="vertical"></ElDivider>
          <ElButton type="warning" plain @click="onUpdateMotorClick">
            更新电机
          </ElButton>
          <ElButton type="danger" plain @click="onDeleteMotorClick">
            删除电机
          </ElButton>
        </div>
        <div class="motor-container" v-if="motors?.length > 0">
          <ElScrollbar class="motors-container">
            <div class="motors-container">
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
          <div class="motor-setting-container">
            <MotorProperty :motor="currentMotor"></MotorProperty>
          </div>
        </div>
        <ElEmpty v-else description="先添加电机"></ElEmpty>
      </div>
    </template>
  </ContentLayout>

  <AddMotor
    v-model="showAddMotor"
    @finished="addMotorFinished"
    :facility-id="currentFacility?.['id']"
  >
  </AddMotor>

  <UpdateMotor
    v-model="showUpdateMotor"
    @finished="updateMotorFinished"
    :motor="currentMotor"
    :facility-id="currentFacility?.['id']"
  >
  </UpdateMotor>

  <DeleteMotor
    v-model="showDeleteMotor"
    @finished="deleteMotorFinished"
    :motor="currentMotor"
    :facility-id="currentFacility?.['id']"
  >
  </DeleteMotor>
</template>

<style scoped lang="scss">
.container {
  flex: 1;
  display: flex;
  flex-flow: column;
  grid-gap: 1rem;
  overflow: auto;

  > .motor-container {
    flex: 1;
    display: flex;
    flex-flow: row;
    overflow: auto;

    .motors-container {
      display: flex;
      flex-flow: column;
      grid-gap: 1rem;

      > .motor {
        margin-right: 1rem;
      }
    }

    > .motor-setting-container {
      flex: 1;
      overflow: auto;
      display: flex;
      flex-flow: column;
    }
  }
}
</style>

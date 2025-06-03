<script setup>
import { ref, defineEmits } from 'vue';
import { updateMotorBindingApi } from '@/apis/sawingMotor.js'
import {ElMessage,ElForm,ElInput,ElButton,ElFormItem, ElRow, ElCol } from 'element-plus'


const props = defineProps(['motor']);
const firstMotor = props.motor[0];
// console.log(firstMotor)
const emit = defineEmits(['submit']);
const loadingUpdateMotor = ref(false)
const handleSubmit = async () => {  
  await updateMotor()
  console.log('子组件提交的数据：', form.value);
}
const updateMotor = async () => {
  if (loadingUpdateMotor.value) {
    return
  }
  loadingUpdateMotor.value = true
  try {
    await updateMotorBindingApi(form.value)
    ElMessage({
      type: 'success',
      message: '更新阈值成功',
    })
    emit('submit',form.value)
  } catch (e) {
    console.error(e)
    ElMessage({
      type: 'error',
      message: '更新阈值失败',
    })
  } finally {
    loadingUpdateMotor.value = false
  }
}
const form = ref({
  bindingType:firstMotor.bindingType,
  connectionName:firstMotor.connectionName,
  connectorInstance:firstMotor.connectorInstance,
  dataPoint:firstMotor.dataPoint,
  id:firstMotor.id,
  motorId:firstMotor.motorId,
  name:firstMotor.name,
  max: firstMotor.max,
  min: firstMotor.min,
  maxWarning: firstMotor.maxWarning,
  minWarning: firstMotor.minWarning,
  maxAlarm: firstMotor.maxAlarm,
  minAlarm: firstMotor.minAlarm,
});

// const handleSubmit = () => {
//   // console.log('子组件提交的数据：', form.value);
//   emit('submit', form.value);
// };
</script>

<template>
    <div>
        <ElForm>
            <ElRow>
                <ElCol :span="6">
                    <ElFormItem label="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;正常"></ElFormItem>
                </ElCol>
                <ElCol :span="5">
                    <ElInput v-model="form.min"  class="centered-input"></ElInput>
                </ElCol>
                <ElCol :span="2" class="centered">
                    -
                </ElCol>
                <ElCol :span="5">
                    <ElInput v-model="form.max"  class="centered-input"></ElInput>
                </ElCol>
                <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                <div class="circle1"></div>
            </ElRow>
        </ElForm>
        <ElForm>
            <ElRow>
                <ElCol :span="6">
                    <ElFormItem label="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;预警"></ElFormItem>
                </ElCol>
                <ElCol :span="5">
                    <ElInput v-model="form.minWarning" class="centered-input"></ElInput>
                </ElCol>
                <ElCol :span="2" class="centered">
                    -
                </ElCol>
                <ElCol :span="5">
                    <ElInput v-model="form.maxWarning"  class="centered-input"></ElInput>
                </ElCol>
                <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                <div class="circle2"></div>
            </ElRow>
        </ElForm>
        <ElForm>
            <ElRow>
                <ElCol :span="6">
                    <ElFormItem label="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;报警"></ElFormItem>
                </ElCol>
                <ElCol :span="5">
                    <ElInput v-model="form.minAlarm"  class="centered-input"></ElInput>
                </ElCol>
                <ElCol :span="2" class="centered">
                    -
                </ElCol>
                <ElCol :span="5">
                    <ElInput v-model="form.maxAlarm"  class="centered-input"></ElInput>
                </ElCol>
                <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                <div class="circle3"></div>
            </ElRow>
        </ElForm>
            <ElFormItem>
            <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>
            <ElButton type="primary" @click="handleSubmit">保存</ElButton>
            </ElFormItem>
    </div>
</template>

<style scoped>
.centered {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%; /* 确保高度与输入框一致 */
  line-height: 1; /* 调整行高以确保垂直居中 */
  font-size: 3rem; /* 设置字体大小 */
}
.circle1 {
  width: 30px; /* 圆形的宽度 */
  height: 30px; /* 圆形的高度 */
  border-radius: 50%; /* 使元素变为圆形 */
  background-color: green; /* 设置背景颜色为绿色 */
  display: inline-block; /* 使圆形可以与其他元素在同一行显示 */
}
.circle2 {
  width: 30px; /* 圆形的宽度 */
  height: 30px; /* 圆形的高度 */
  border-radius: 50%; /* 使元素变为圆形 */
  background-color: rgb(255, 255, 5); /* 设置背景颜色为绿色 */
  display: inline-block; /* 使圆形可以与其他元素在同一行显示 */
}
.circle3 {
  width: 30px; /* 圆形的宽度 */
  height: 30px; /* 圆形的高度 */
  border-radius: 50%; /* 使元素变为圆形 */
  background-color: red; /* 设置背景颜色为绿色 */
  display: inline-block; /* 使圆形可以与其他元素在同一行显示 */
}
.centered-input .el-input__inner {
  text-align: center; /* 设置输入内容居中 */
}
</style>
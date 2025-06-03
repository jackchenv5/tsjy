<script setup>
import * as xlsx from 'xlsx'
import { ref } from 'vue'

// region 组件
const props = defineProps({
  facilityId: {
    type: Number,
    require: true,
  },
})
const emits = defineEmits(['change'])
// endregion

// region 导入
const uploadRef = ref()
const onUploadChange = (file) => {
  const reader = new FileReader()
  reader.onload = () => {
    if (reader.result) {
      const header = [
        'treePath',
        'name',
        'count',
        'description',
        'partType',
        'maintainCycle',
      ]
      const workBook = xlsx.read(reader.result, { type: 'binary' })
      const firstWorkSheetName = workBook.SheetNames[0]
      const workSheet = workBook.Sheets[firstWorkSheetName]
      const partData = xlsx.utils.sheet_to_json(workSheet, {
        header: header,
      })
      partData.shift()
      for (let i = 0; i < partData.length; i++) {
        partData[i]['facilityId'] = props.facilityId
        if (partData[i]['partType'] === '固件') {
          partData[i]['partType'] = 0
        } else {
          partData[i]['partType'] = 1
        }
      }
      emits('change', partData)
    }
    uploadRef.value.clearFiles()
  }
  reader.readAsBinaryString(file.raw)
}
// endregion
</script>

<template>
  <ElUpload
    :limit="1"
    :auto-upload="false"
    :show-file-list="false"
    accept=".xlsx"
    :on-change="onUploadChange"
    ref="uploadRef"
  >
    <slot></slot>
  </ElUpload>
</template>

<style scoped lang="scss"></style>





<script>

export default {
  name: 'Header',
  data(){
    return {
      images:{
        Logo: require('../assets/AkteLogo.webp'),
      }
    }
  }
}
</script>
<template>
  <div class="header">
    <div><img :src="images.Logo"  data-pos="1"/></div>
  </div>
</template>
<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
  .header{
    width:100%;
    display:flex;
    align-items:center;
  }
  .header img{
    width:40%;
  }

</style>

const fs = require('fs');
const nameCounts = {};

const dt1 = new Date();
console.log('Start time: ' + dt1.getTime());
console.time('Elapsed time: ');

fs.readFile('names.csv', 'utf8', (err, data) => {
  if (err) {
    console.error('Error reading the file:', err);
    return;
  }

  const lines = data.split('\n');

  lines.slice(1).forEach((line) => {
    const columns = line.split(',');
    const name = columns[3]; // ปรับให้ตรงกับตำแหน่งชื่อในไฟล์ CSV

    if (name) { // ตรวจสอบว่าชื่อไม่ว่าง
      nameCounts[name] = (nameCounts[name] || 0) + 1;
    }
  });

  const totalNames = Object.keys(nameCounts).length;

  // หาชื่อที่ซ้ำกันมากที่สุด 10 อันดับ
  const mostFrequent = Object.entries(nameCounts)
    .sort((a, b) => b[1] - a[1])
    .slice(0, 10);

  // หาชื่อที่ซ้ำกันน้อยที่สุด 10 อันดับ
  const leastFrequent = Object.entries(nameCounts)
    .sort((a, b) => a[1] - b[1])
    .slice(0, 10);

  console.log(`Total unique names: ${totalNames}`);

  console.log('Most duplicated names:');
  mostFrequent.forEach(([name, count], index) => {
    console.log(`${index + 1}. ${name} = ${count}`);
  });

  console.log('Least duplicated names:');
  leastFrequent.forEach(([name, count], index) => {
    console.log(`${index + 1}. ${name} = ${count}`);
  });

  console.log('Finished time: ' + new Date().getTime());
  console.timeEnd('Elapsed time: ');
});

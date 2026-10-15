  const euroFormatter = new Intl.NumberFormat('nl-NL', {
    style: 'currency',
    currency: 'EUR',
  });

  const numberFormatter = new Intl.NumberFormat('nl-NL');

export function formatWh(num:number){
    const factorG = 1000_000;
    const factorM = 1000;
   

  if (num > factorG)
  {
    return `${Math.round(num/factorG)} GWh`;
  }
  else if (num > factorM) {
    return `${Math.round(num/factorM)} mWh`;
  }
  return `${num} kWh`;
}
export function pad(num:(number | string), size:number) {
    let outNum = num.toString();
    while (outNum.length < size) outNum = "0" + outNum;
    return outNum;
}

export function formatEuro(num:number){
  return euroFormatter.format(num); 
}

export function formatNumber(num:number):string{
  return numberFormatter.format(num);
}
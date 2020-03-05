import pandas as pd
import numpy as np
from datetime import datetime
import glob
import re

################################################################
# 변수
################################################################
## CSV 폴더
sourceCsvFolder = './datasets/**/*.csv'
## 결과 파일명
resultFileName = f'./MultiResults_{datetime.now().strftime("%Y%m%d_%H%M%S")}.csv'
## 쿼리 조건
conditions = [
    ['avg90',    0.05, 0.05],  # 상위  5%, 하위  5%를 제외한 90%의 일일 평균값
    ['avg90H10', 0.05, 0.85],  # 상위  5%, 하위 85%를 제외한 10%의 일일 평균값
    ['avg90L10', 0.85, 0.05],  # 상위 85%, 하위  5%를 제외한 10%의 일일 평균값
]
## 장비의 총 메모리
machineTotalMemory = {
    "WIN-3OPFVMF4N3A": 8,      # 호스트 네임, 메모리 용량(GByte)
    "DESKTOP-E9MCJ4E": 16,     # 라인 내 컴퓨터를 밑으로 추가
}

################################################################
# 소스코드
################################################################
def ReadSingleCSV(filename):
    df = pd.read_csv(filename,parse_dates=[0], na_values=[' '])
    # df에서 colums 추출하기
    col = list(map(lambda x:x.replace('\\','/'), df.columns))

    # 설비 명 찾기
    machineName = re.match('^//([^/]*)/', col[2]).group(1)

    # 첫번째 열 이름 바꾸기
    col[0] = "Date"

    # Column 내 장비명을 삭제하여 정규화하기
    col2 = map(lambda x: re.sub('^//([^/]*)/', '', x), col)

    # df의 Columns 이름 변경
    df.columns = col2

    #df에 machineName 컬럼 추가
    df["MachineName"] = machineName 
    df.set_index(["Date", "MachineName"], inplace=True)

    ret_cond = []
    for x in conditions:
        df90 = df.where(df <= df.quantile(q=(1-x[1]))).where(df >= df.quantile(q=x[2])).unstack().resample('1h').mean().interpolate()
        df90["Type"] = x[0]
        df90.set_index("Type", append=True, inplace=True )
        ret_cond.append(df90.unstack())
    
    # 결과 합치기
    ret = pd.concat(ret_cond)
    return ret


# datasets 하위 여러 csv 파일을 읽어서 결과를 생성
result = pd.DataFrame()

files = glob.glob(sourceCsvFolder, recursive=True)

for idx, x in enumerate(files):
    try:
        df = ReadSingleCSV(x)
        result = pd.concat([df, result])
    except Exception as ex:
        print (f'Runtime Error: {ex}')
    print (f'{idx+1}/{len(files)} : {x} ')

result1 = result.resample('1d').mean().stack(level=1).stack(level=0)
result2 = result1.reset_index()

col = result2.columns.to_list()
col.insert(3, 1)
col.insert(3, 0)

for index, row in result2.iterrows():
    if(row[2] == 'Memory/Available MBytes'):
        if row[1] in machineTotalMemory:
            newRow = [row[0], row[1], "Memory/Available Percent"]
            for x in conditions:
                newRow.append(row[x[0]] / (machineTotalMemory[row[1]] * 1024))
            print(newRow)
            newDf = pd.DataFrame([newRow], columns=result2.columns)
            result2 = result2.append(newDf, ignore_index=True)

result3 = result2.join(result2["level_2"].str.split('/', 1, expand=True))

result4 = result3[col]

result5 = result4.drop('level_2', 1)

result5.to_csv(resultFileName)

print(result4)
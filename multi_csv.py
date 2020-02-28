import pandas as pd
import numpy as np
import glob
import re

################################################################
# 변수
################################################################
## CSV 폴더
sourceCsvFolder = './datasets/**/*.csv'
## 결과 파일명
resultFileName = "./MultiResults.csv"
## 쿼리 조건
conditions = [
    ['avg90',    0.05, 0.05],  # 상위  5%, 하위  5%를 제외한 90%의 일일 평균값
    ['avg90H10', 0.05, 0.85],  # 상위  5%, 하위 85%를 제외한 10%의 일일 평균값
    ['avg90L10', 0.85, 0.05],  # 상위 85%, 하위  5%를 제외한 10%의 일일 평균값
]

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
    date = df["Date"].mean().strftime('%Y-%m-%d')
    df = df.drop("Date", axis=1)


    ret_cond = []
    for x in conditions:
        df90 = df.where(df <= df.quantile(q=(1-x[1]))).where(df >= df.quantile(q=x[2])).mean().to_frame(machineName)
        # df에 파일 컬럼 추가
        df90["Type"] = x[0]
        df90.set_index(["Type"], append=True, inplace=True )
        ret_cond.append(df90)
    
    # 결과 합치기
    ret = pd.concat(ret_cond)
    ret = ret.unstack().stack(level=0)
    ret['Date'] = date
    ret['Filename'] = filename
    ret.set_index(["Date", 'Filename'], append=True, inplace=True )
    ret = ret.unstack(level=0).unstack(level=0).unstack(level=1).stack(level=2).stack(level=2).stack(level=1)
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

result.to_csv(resultFileName)
print(result)




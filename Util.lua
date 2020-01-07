function Initialize()
  dofile(SKIN:GetVariable('@')..'lib\\json.lua')
end

function Update()
  PrMeasure = SKIN:GetMeasure('PRStatusMeasure')
  print(PrMeasure:GetStringValue())
  -- local resp = jsonDecode(PrMeasure:GetStringValue())
  -- return resp.value
end